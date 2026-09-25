using Core.DTO.SolocitudCotizaciones;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Azure;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Firebase;
using Core.Interfaces.Requests;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.Cotizaciones;
using Google;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Core.Services.SolicitudCotizaciones
{
	public class CotizacionYonkeService : ICotizacionYonkeService
	{
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWork;
		private readonly IAlmacenadorArchivos _azureBlobService;

		private readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;

		public readonly ICurrentUserService _currentUserService;

		private readonly IFirebaseNotificationService _firebaseNotificationService;

		

		public CotizacionYonkeService(IUnitOfWorkSolicitudYonkes unitOfWork, 
									  IAlmacenadorArchivos almacenadorArchivos,
									  IUnitOfWorkSolicitudes unitOfWorkSolicitudes,
									  ICurrentUserService currentUserService,
									  IFirebaseNotificationService firebaseNotificationService)
		{
			_unitOfWork = unitOfWork;
			_azureBlobService = almacenadorArchivos;
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;

			_currentUserService = currentUserService;

			_firebaseNotificationService = firebaseNotificationService;
		}

		public async Task<Guid> RegistrarCotizacionAsync(Guid solicitudYonkeGuidId, 
														RegistrarCotizacionRequest request,
														CancellationToken cancellationToken)
		{
			// ==========================================
			// 1. Validaciones básicas
			// ==========================================

			if (solicitudYonkeGuidId == Guid.Empty)
				throw new BusinessException(
					"El identificador de la solicitud no es válido.");

			if (request == null)
				throw new BusinessException(
					"La información de la cotización es requerida.");

			// ==========================================
			// 2. Validar cantidad de imágenes
			// ==========================================

			var imagenes = request.Imagenes ?? new List<IFormFile>();

			const int maxImagenes = 3;

			if (imagenes.Count > maxImagenes)
			{
				throw new BusinessException(
					$"Solo se permiten {maxImagenes} imágenes por cotización.");
			}

			// ==========================================
			// 3. Validar información de la cotización
			// ==========================================

			if (request.Precio < 0)
			{
				throw new BusinessException(
					"El precio de la cotización no puede ser negativo.");
			}

			if (request.TieneGarantia && request.DiasGarantia <= 0)
			{
				throw new BusinessException(
					"Debes especificar los días de garantía.");
			}

			if (request.TiempoEntregaDias.HasValue &&
				request.TiempoEntregaDias.Value < 0)
			{
				throw new BusinessException(
					"El tiempo de entrega no puede ser negativo.");
			}

			if (request.EnvioDisponible &&
				   request.CostoEnvio.HasValue &&
				   request.CostoEnvio.Value < 0)
			{
				throw new BusinessException(
					"El costo de envío no puede ser negativo.");
			}


			// ==========================================
			// 4. Validar imágenes antes de guardar BD
			// ==========================================

			ValidarImagenesCotizacion(imagenes);

			// ==========================================
			// 5. Validar que no exista cotización
			// ==========================================

			var existe = await _unitOfWork
				.SolicitudCotizacionRepository
				.ExisteCotizacionAsync(solicitudYonkeGuidId);

			if (existe)
			{
				throw new BusinessException(
					"La solicitud ya cuenta con una cotización registrada.");
			}


			// ==========================================
			// 6. Obtener SolicitudYonke + Solicitud
			// ==========================================

			var solicitudYonke =
				await _unitOfWork
					.SolicitudYonkeRepository
					.ObtenerConSolicitudPorGuidAsync(
						solicitudYonkeGuidId);

			if (solicitudYonke == null)
			{
				throw new BusinessException(
					"La solicitud enviada al yonke no existe.");
			}

			if (solicitudYonke.Solicitudes == null)
			{
				throw new BusinessException(
					"No fue posible obtener la solicitud original.");
			}

			// ==========================================
			// 7. Validar cliente
			// ==========================================

			var usuarioClienteId =
				solicitudYonke.Solicitudes.UsuarioId;

			if (usuarioClienteId == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al cliente de la solicitud.");
			}

			// ==========================================
			// 8. Obtener usuario actual
			// ==========================================

			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			// ==========================================
			// 9. Crear cotización
			// ==========================================

			var cotizacion = new Core.Entitys.SolicitudCotizaciones
				{
					GuidId = Guid.NewGuid(),
					FechaCreacion = DateTime.UtcNow,
					UsuarioId = usuarioId.Value,
					SolicitudYonkeGuidId = solicitudYonkeGuidId,
					MarcaId = request.MarcaId,
					Precio = request.Precio,
					Disponible = request.Disponible,
					EsNueva = request.EsNueva,
					NumeroParte = string.IsNullOrWhiteSpace(request.NumeroParte)
							? null
							: request.NumeroParte.Trim(),
					Comentarios = string.IsNullOrWhiteSpace(request.Comentarios)
							? null
							: request.Comentarios.Trim(),
					TieneGarantia = request.TieneGarantia,
					DiasGarantia = request.TieneGarantia
							? request.DiasGarantia
							: 0,
					EnvioDisponible = request.EnvioDisponible,
					
					TiempoEntregaDias = request.TiempoEntregaDias,
					Activo = true,
					EstatusId = (int)SolicitudYonkeEstatusEnum.Cotizada
				};

			// ==========================================
			// 10. Agregar cotización
			// ==========================================

			await _unitOfWork.SolicitudCotizacionRepository.AgregarAsync(cotizacion);


			// ==========================================
			// 11. Subir imágenes
			// ==========================================

			if (imagenes.Count > 0)
			{
				await GuardarImagenesCotizacionAsync(
					cotizacion,
					imagenes,
					cancellationToken);
			}

			// ==========================================
			// 12. Actualizar SolicitudYonke
			// ==========================================

			solicitudYonke.EstatusId =
				(int)SolicitudYonkeEstatusEnum.Cotizada;

			// ==========================================
			// 13. Actualizar Solicitud original
			// ==========================================

			solicitudYonke.Solicitudes.EstatusSolicitudId = (int)EstatusSolicitudEnum.Cotizada;


			// ==========================================
			// 14. Guardar el historial
			// ==========================================
			if (solicitudYonke.Yonkes == null)
			{
				throw new BusinessException(
					"No fue posible obtener la información del yonke.");
			}

			var historial = new SolicitudesHistorials
			{
				SolicitudGuidId = solicitudYonke.Solicitudes.GuidId,
				EstatusSolicitudId = (int)EstatusSolicitudEnum.Cotizada,
				Fecha = DateTime.UtcNow,
				UsuarioId = usuarioId.Value,
				Comentarios =
					$"Cotizada por {solicitudYonke.Yonkes.Nombre}, " +
					$"GuidId: {cotizacion.GuidId}."
			};

			await _unitOfWorkSolicitudes
				.solicitudHistorialRepository
				.AddHistorialAsync(historial);

			// ==========================================
			// 14. Guardar todos los cambios SQL
			// ==========================================

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			await _unitOfWorkSolicitudes.SaveChangesAsync();

			// ==========================================
			// 15. Notificar cliente
			// ==========================================

			/*
			await _firebaseNotificationService.SendMulticastAsync(
				tokens,
				"Nueva cotización",
				"Has recibido una nueva cotización para tu solicitud.",
				new Dictionary<string, string>
				{
					["tipo"] = "nueva_cotizacion",
					["cotizacionGuidId"] =
						cotizacion.GuidId.ToString(),

					["solicitudYonkeGuidId"] =
						solicitudYonkeGuidId.ToString(),

					["solicitudGuidId"] =
						solicitudYonke.SolicitudGuidId.ToString()
				},
				cancellationToken);
			*/

			return cotizacion.GuidId;
		}

		public async Task ActualizarCotizacionAsync(Guid cotizacionGuidId,
													RegistrarCotizacionRequest request,
													CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId
				?? throw new UnauthorizedAccessException(
				"No fue posible identificar al usuario.");

			if (userId == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			var cotizacion = await _unitOfWork.SolicitudCotizacionRepository
				.ObtenerPorGuidAsync(cotizacionGuidId, cancellationToken);

			if (cotizacion == null)
				throw new BusinessException("La cotización no existe.");

			if (cotizacion.UsuarioId != userId)
				throw new BusinessException("No tienes permisos para modificar esta cotización.");

			cotizacion.MarcaId = request.MarcaId;
			cotizacion.Precio = request.Precio;
			cotizacion.Disponible = request.Disponible;
			cotizacion.EsNueva = request.EsNueva;
			cotizacion.NumeroParte = request.NumeroParte;
			cotizacion.Comentarios = request.Comentarios;
			cotizacion.TieneGarantia = request.TieneGarantia;
			cotizacion.DiasGarantia = request.DiasGarantia;
			cotizacion.EnvioDisponible = request.EnvioDisponible;
			cotizacion.TiempoEntregaDias = request.TiempoEntregaDias;

			await _unitOfWork.SolicitudCotizacionRepository
				.ActualizarAsync(cotizacion);

			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<Entitys.SolicitudCotizaciones?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken)
		{
			return await _unitOfWork.SolicitudCotizacionRepository
			.ObtenerPorGuidAsync(guidId, cancellationToken);
		}





		/// <summary>
		/// Metodo para validar las imagnees antes de grabar
		/// </summary>
		/// <param name="imagenes"></param>
		/// <exception cref="BusinessException"></exception>
		private static void ValidarImagenesCotizacion(IReadOnlyCollection<IFormFile> imagenes)
		{
			const long maxSize =
				5 * 1024 * 1024; // 5 MB

			var extensionesPermitidas =
				new HashSet<string>(
					StringComparer.OrdinalIgnoreCase)
				{
				".jpg",
				".jpeg",
				".png"
				};

			var tiposPermitidos =
				new HashSet<string>(
					StringComparer.OrdinalIgnoreCase)
				{
				"image/jpeg",
				"image/png"
				};

			foreach (var imagen in imagenes)
			{
				if (imagen == null)
				{
					throw new BusinessException(
						"Se recibió una imagen inválida.");
				}

				if (imagen.Length <= 0)
				{
					throw new BusinessException(
						$"La imagen '{imagen.FileName}' está vacía.");
				}

				if (imagen.Length > maxSize)
				{
					throw new BusinessException(
						$"La imagen '{imagen.FileName}' excede el tamaño máximo de 5 MB.");
				}

				var extension =
					Path.GetExtension(imagen.FileName);

				if (string.IsNullOrWhiteSpace(extension) ||
					!extensionesPermitidas.Contains(extension))
				{
					throw new BusinessException(
						$"La imagen '{imagen.FileName}' tiene una extensión no permitida.");
				}

				var contentType =
					imagen.ContentType?.Trim();

				if (string.IsNullOrWhiteSpace(contentType) ||
					!tiposPermitidos.Contains(contentType))
				{
					throw new BusinessException(
						$"La imagen '{imagen.FileName}' tiene un tipo de archivo no permitido.");
				}
			}
		}



		/// <summary>
		/// Guardar las imanges de la cotizacion que hace el yonke
		/// </summary>
		/// <param name="cotizacion"></param>
		/// <param name="imagenes"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task GuardarImagenesCotizacionAsync(Core.Entitys.SolicitudCotizaciones cotizacion,
		IReadOnlyList<IFormFile> imagenes,
		CancellationToken cancellationToken)
		{
			const string contenedor = "cotizacionimagenes";

			for (var i = 0; i < imagenes.Count; i++)
			{
				var imagen = imagenes[i];

				var orden = i + 1;

				var extension =
					Path.GetExtension(imagen.FileName)
						.ToLowerInvariant();

				var nombreArchivo = $"{Guid.NewGuid():N}{extension}";

				var rutaBlob = $"cotizacion/{cotizacion.GuidId}/{nombreArchivo}";

				await using var stream = new MemoryStream();

				await imagen.CopyToAsync(stream, cancellationToken);

				var contenido = stream.ToArray();

				var contentType = imagen.ContentType.Trim();

				var url = await _azureBlobService.GuardarArchivo(
						contenido,
						extension,
						contenedor,
						rutaBlob,
						contentType);

				var imagenCotizacion = new Entitys.SolicitudCotizacionesImagenes
					{
						GuidId = Guid.NewGuid(),
						CotizacionGuidId = cotizacion.GuidId,
						UrlImagen = url,
						RutaBlob = rutaBlob,
						EsPrincipal = orden == 1, 
						Orden = orden,
						CreateAt = DateTime.UtcNow
					};

				await _unitOfWork.solicitudCotizacionImagenesRepository.AgregarAsync(imagenCotizacion);
			}
		}


	}
}
