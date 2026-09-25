using Core.DTO.SolicitudMenssages;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Firebase;
using Core.Interfaces.Negocio;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.CotizacionesImagenes;
using Core.Interfaces.SingalR;

namespace Core.Services.SolicitudCotizaciones
{
	public class SolicitudCotizacionMensajeService : ISolicitudCotizacionMensajeService
	{
		private readonly IUnitOfWorkNegocio _unitOfWorkNegocio; //Yonke
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes; //Yonke Solicitufdes
		private readonly IChatNotificationService _chatNotificationService; //Signal 		
		private readonly IFirebaseNotificationService _firebaseNotificationService; //Firebase
		

		public SolicitudCotizacionMensajeService(IUnitOfWorkNegocio unitOfWorkNegocio,
												 IUnitOfWorkSolicitudYonkes unitOfWorkSolicitudYonkes,
												 IChatNotificationService chatNotificationService,
												 IFirebaseNotificationService firebaseNotificationService)
		{
			_unitOfWorkNegocio = unitOfWorkNegocio;
			_unitOfWorkSolicitudYonkes = unitOfWorkSolicitudYonkes;
			_chatNotificationService = chatNotificationService;
			_firebaseNotificationService = firebaseNotificationService;
		}

		public async Task<SolicitudCotizacionMensajeDTO> EnviarMensajeAsync(RegistrarMensajeCotizacionRequest request, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// 1. Validar mensaje
			// ==========================================

			if (string.IsNullOrWhiteSpace(request.Mensaje))
			{
				throw new Exception("El mensaje es obligatorio.");
			}


			// ==========================================
			// 2. Obtener cotización
			// ==========================================

			var cotizacion =
				await _unitOfWorkSolicitudYonkes
					.SolicitudCotizacionRepository
					.ObtenerPorGuidAsync(
						request.SolicitudCotizacionGuidId,
						cancellationToken);

			if (cotizacion == null)
			{
				throw new Exception("No se encontró la cotización.");
			}


			// ==========================================
			// 3. Validar cotización activa
			// ==========================================

			if (!cotizacion.Activo)
			{
				throw new Exception("La cotización ya no está activa.");
			}


			// ==========================================
			// 4. Determinar remitente
			// ==========================================

			int tipoRemitenteId;

			if (cotizacion.UsuarioId == usuarioId)
			{
				tipoRemitenteId = (int)TipoRemitenteCotizacionEnum.Cliente;
			}
			else if (
				cotizacion.SolicitudYonkes?.Solicitudes?.UsuarioId == usuarioId)
			{
				tipoRemitenteId = (int)TipoRemitenteCotizacionEnum.Yonke;
			}
			else
			{
				throw new UnauthorizedAccessException("No tienes autorización para enviar mensajes en esta cotización.");
			}


			// ==========================================
			// 5. Crear mensaje
			// ==========================================

			var mensaje = new SolicitudCotizacionMensajes
			{
				GuidId = Guid.NewGuid(),
				SolicitudCotizacionGuidId = request.SolicitudCotizacionGuidId,
				UsuarioId = usuarioId, 
				TipoRemitenteId = tipoRemitenteId,
				Mensaje = request.Mensaje.Trim(),
				Leido = false, 
				FechaCreacion = DateTime.UtcNow
			};


			// ==========================================
			// 6. Guardar mensaje
			// ==========================================

			await _unitOfWorkSolicitudYonkes
				.SolicitudCotizacionMensajeRepository
				.AgregarAsync(
					mensaje,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);


			// ==========================================
			// 7. Obtener SolicitudYonke
			// ==========================================

			var solicitudYonke =
				await _unitOfWorkSolicitudYonkes
					.SolicitudYonkeRepository
					.GetDataSolicitudyonkeByGuidId(
						cotizacion.SolicitudYonkeGuidId);

			if (solicitudYonke == null)
			{
				throw new Exception("No se encontró la solicitud del yonke.");
			}


			// ==========================================
			// 8. Obtener tokens Firebase
			// ==========================================

			var tokens =
				await _unitOfWorkNegocio
					.YunkeRepository
					.ObtenerTokensFirebaseAsync(
						new List<Guid>
						{
					solicitudYonke.YonkeGuidId
						});


			// ==========================================
			// 9. Firebase
			// ==========================================

			if (tokens.Any())
			{
				await _firebaseNotificationService
					.EnviarNotificacionAsync(
						tokens,

						tipoRemitenteId ==
							(int)TipoRemitenteCotizacionEnum.Cliente
							? "Nuevo mensaje del cliente"
							: "Nuevo mensaje del yonke",

						mensaje.Mensaje,

						new Dictionary<string, string>
						{
							["tipo"] = 
								"nuevo_mensaje",

							["cotizacionGuidId"] =
								mensaje
									.SolicitudCotizacionGuidId
									.ToString(),

							["mensajeGuidId"] =
								mensaje.GuidId
									.ToString(),

							["tipoRemitenteId"] =
								mensaje.TipoRemitenteId
									.ToString()
						},

						cancellationToken);
			}


			// ==========================================
			// 10. SignalR
			// ==========================================

			await _chatNotificationService
				.EnviarMensajeAsync(
					mensaje.SolicitudCotizacionGuidId,

					new
					{
						mensaje.GuidId,
						mensaje.SolicitudCotizacionGuidId,
						mensaje.UsuarioId,
						mensaje.TipoRemitenteId,
						mensaje.Mensaje,
						mensaje.FechaCreacion
					},
					cancellationToken
				);


			// ==========================================
			// 11. Retornar mensaje
			// ==========================================

			return new SolicitudCotizacionMensajeDTO
			{
				GuidId =
					mensaje.GuidId,
				SolicitudCotizacionGuidId = mensaje.SolicitudCotizacionGuidId,			 
				UsuarioId = mensaje.UsuarioId,
				TipoRemitenteId = mensaje.TipoRemitenteId,
				TipoRemitente = mensaje.TipoRemitenteId ==
					(int)TipoRemitenteCotizacionEnum.Cliente
						? "Cliente"
						: "Yonke",
				Mensaje = mensaje.Mensaje,
				Leido = mensaje.Leido,
				FechaLectura = mensaje.FechaLectura, 
				FechaCreacion = mensaje.FechaCreacion
			};
		}

		public async Task<List<SolicitudCotizacionMensajeDTO>>ObtenerMensajesAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			var cotizacion = await _unitOfWorkSolicitudYonkes.SolicitudCotizacionRepository
					.ObtenerPorGuidAsync(
						solicitudCotizacionGuidId,
						cancellationToken);

			if (cotizacion == null)
			{
				throw new Exception(
					"No se encontró la cotización.");
			}

			var mensajes =
				await _unitOfWorkSolicitudYonkes
					.SolicitudCotizacionMensajeRepository
					.ObtenerPorCotizacionAsync(
						solicitudCotizacionGuidId,
						cancellationToken);

			return mensajes
				.Select(x => new SolicitudCotizacionMensajeDTO
				{
					GuidId = x.GuidId,
					SolicitudCotizacionGuidId =	x.SolicitudCotizacionGuidId,
					UsuarioId = x.UsuarioId,
					TipoRemitenteId = x.TipoRemitenteId,
					TipoRemitente =	x.TipoRemitenteId ==
						(int)TipoRemitenteCotizacionEnum.Cliente
							? "Cliente"
							: "Yonke",
					Mensaje = x.Mensaje,
					Leido = x.Leido,
					FechaLectura = x.FechaLectura,
					FechaCreacion = x.FechaCreacion
				}).ToList();
		}

		public async Task MarcarComoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			await _unitOfWorkSolicitudYonkes
				.SolicitudCotizacionMensajeRepository
				.MarcarComoLeidosAsync(
					solicitudCotizacionGuidId,
					usuarioId,
					cancellationToken);
		}

		public async Task<int> ObtenerCantidadNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{			
			return await _unitOfWorkSolicitudYonkes
				.SolicitudCotizacionMensajeRepository
				.ObtenerCantidadNoLeidosAsync(
					solicitudCotizacionGuidId,
					usuarioId,
					cancellationToken);
		}
	}
}
