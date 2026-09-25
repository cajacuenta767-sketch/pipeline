using AutoMapper;
using Core.DTO.Solicitudes.imagnees;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Azure;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.imagenes;
using Microsoft.AspNetCore.Http;

namespace Core.Services.Requests
{
	public class SolicitudesImagenesService : ISolicitudesImagenesService
	{
		private readonly IAlmacenadorArchivos _azureBlobService;
		private readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;

		private readonly IMapper _mapper;

		public readonly ICurrentUserService _currentUserService;
		public SolicitudesImagenesService(IAlmacenadorArchivos azureBlobService,
										  IUnitOfWorkSolicitudes unitOfWorkSolicitudes,
										  IMapper mapper,
										  ICurrentUserService currentUserService)
		{
			_azureBlobService = azureBlobService;
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;
			_mapper = mapper;
			_currentUserService = currentUserService;
		}


		public async Task<IList<Guid>> AgregarImagenesAsync(Guid solicitudGuidId, IList<IFormFile> imagenes, CancellationToken cancellationToken)
		{
			if (imagenes == null || !imagenes.Any())
				throw new BusinessException("Debe seleccionar al menos una imagen.");

			const int maxImagenes = 3;

			if (imagenes.Count > maxImagenes)
				throw new BusinessException($"Solo se permiten {maxImagenes} imágenes por solicitud.");

			var solicitud = await _unitOfWorkSolicitudes
				.SolicitudesRepository
				.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
				throw new BusinessException("La solicitud no existe.");

			var extensionesPermitidas = new HashSet<string>
			{
				".jpg",
				".jpeg",
				".png"
			};

			var tiposPermitidos = new HashSet<string>
			{
				"image/jpeg",
				"image/png"
			};

			const long maxSize = 6 * 1024 * 1024; //  6 MB

			//aqui defino el nombre del contenedor
			const string contenedor = "solicitudimagenes";

			var guids = new List<Guid>();

			using var transaction = await _unitOfWorkSolicitudes.BeginTransactionAsync();

			try
			{
				foreach (var imagen in imagenes)
				{
					var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

					if (!extensionesPermitidas.Contains(extension))
						throw new BusinessException(
							$"La imagen '{imagen.FileName}' tiene una extensión no permitida.");

					if (!tiposPermitidos.Contains(imagen.ContentType))
						throw new BusinessException(
							$"La imagen '{imagen.FileName}' tiene un tipo de archivo no permitido.");

					if (imagen.Length > maxSize)
						throw new BusinessException(
							$"La imagen '{imagen.FileName}' excede el tamaño máximo de 5 MB.");

					using var memoryStream = new MemoryStream();

					await imagen.CopyToAsync(memoryStream);

					var contenido = memoryStream.ToArray();
									
					//ruta de blob azure
					var rutaBlob = $"solicitud/{solicitudGuidId}/{Guid.NewGuid()}{extension}";

					var url = await _azureBlobService.GuardarArchivo(
						contenido,
						extension,
						contenedor,
						rutaBlob,
						imagen.ContentType);

					var entity = new SolicitudesImagenes
					{
						GuidId = Guid.NewGuid(),
						SolicitudGuidId = solicitud.GuidId,
						UrlImagen = url,
						RutaBlob = rutaBlob,
						FechaCreacion = DateTime.UtcNow
					};

					await _unitOfWorkSolicitudes.solicitudImagenesRepository.AgregarImagenAsync(entity);

					guids.Add(entity.GuidId);
				}




				// ======================================
				// Guardar historial
				// ======================================

				var usuarioId = _currentUserService.UserId;

				if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
				{
					throw new BusinessException(
						"No fue posible identificar al usuario.");
				}

				var historial = new SolicitudesHistorials
				{
					SolicitudGuidId = solicitud.GuidId,
					EstatusSolicitudId = solicitud.EstatusSolicitudId,
					Fecha = DateTime.UtcNow,
					UsuarioId = usuarioId.Value,
					Comentarios = "Se agregaron imagenes."
				};

				await _unitOfWorkSolicitudes.solicitudHistorialRepository.AddHistorialAsync(historial);

				await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);


				await transaction.CommitAsync();

				return guids;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<SolicitudesImagenes_DTO?> GetImagenAsync(Guid imagenGuidId)
		{
			return await _unitOfWorkSolicitudes.solicitudImagenesRepository.GetImagenByGuidIdAsync(imagenGuidId);
		}

		public async Task<IList<SolicitudesImagenes_List_DTO>> GetImagenesAsync(Guid solicitudGuidId)
		{
			return await _unitOfWorkSolicitudes.solicitudImagenesRepository.GetImagenesBySolicitudAsync(solicitudGuidId);
		}

		public async Task<bool> EliminarImagenAsync(Guid imagenGuidId)
		{
			var imagen = await _unitOfWorkSolicitudes.solicitudImagenesRepository.GetByGuidId(imagenGuidId);

			if (imagen == null)
				return false;

			using var transaction = await _unitOfWorkSolicitudes.BeginTransactionAsync();

			try
			{
				// borrar Azure
				await _azureBlobService.BorrarArchivo(imagen.RutaBlob, "solicitudimagenes");

				// borrar registro SQL
				await _unitOfWorkSolicitudes.solicitudImagenesRepository.EliminarImagenAsync(imagenGuidId);



				// ======================================
				// Guardar historial
				// ======================================

				var usuarioId = _currentUserService.UserId;

				if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
				{
					throw new BusinessException(
						"No fue posible identificar al usuario.");
				}

				var historial = new SolicitudesHistorials
				{
					SolicitudGuidId = imagen.GuidId,
					EstatusSolicitudId = imagen.Solicitudes.EstatusSolicitudId,
					Fecha = DateTime.UtcNow,
					UsuarioId = usuarioId.Value,
					Comentarios = "Se elimino imagen."
				};

				await _unitOfWorkSolicitudes.solicitudHistorialRepository.AddHistorialAsync(historial);

				await _unitOfWorkSolicitudes.SaveChangesAsync();

				await transaction.CommitAsync();

				return true;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<IList<SolicitudesImagenes_List_DTO>> GetImagenesBySolicitudAsync(Guid solicitudGuidId)
		{
			return await _unitOfWorkSolicitudes.solicitudImagenesRepository.GetImagenesBySolicitudAsync(solicitudGuidId);
		}

		public async Task<SolicitudesImagenes_DTO?> GetImagenByGuidIdAsync(Guid imagenGuidId)
		{
			return await _unitOfWorkSolicitudes.solicitudImagenesRepository.GetImagenByGuidIdAsync(imagenGuidId);
		}

		
	}
}
