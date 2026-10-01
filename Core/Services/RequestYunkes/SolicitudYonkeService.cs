using Core.DTO.SolicitudYonkes;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Firebase;
using Core.Interfaces.Negocio;
using Core.Interfaces.Requests;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.Solicitudes;
using Core.Statics;
using Microsoft.Extensions.Logging;

namespace Core.Services.RequestYonkes
{
	public class SolicitudYonkeService : ISolicitudYonkeService
	{
		private readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;
		private readonly IUnitOfWorkNegocio _unitOfWorkNegocio;
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes;
		private readonly IFirebaseNotificationService _firebaseNotificationService;

		private readonly ILogger<SolicitudYonkeService> _logger;

		private readonly ICurrentUserService _currentUserService;


		public SolicitudYonkeService(IUnitOfWorkSolicitudes unitOfWorkSolicitudes,
									 IUnitOfWorkNegocio unitOfWorkNegocio,
									 IUnitOfWorkSolicitudYonkes unitOfWorkSolicitudYonkes,
									 IFirebaseNotificationService firebaseNotificationService,
									 ILogger<SolicitudYonkeService> logger,
									 ICurrentUserService currentUserService)
		{
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;
			_unitOfWorkNegocio = unitOfWorkNegocio;
			_unitOfWorkSolicitudYonkes = unitOfWorkSolicitudYonkes;
			_firebaseNotificationService = firebaseNotificationService;
			_logger = logger;
			
			_currentUserService = currentUserService;
		}


		public async Task<int> EnviarSolicitudAsync(Guid solicitudGuidId, CancellationToken cancellationToken)
		{
			await using var transaction = await _unitOfWorkSolicitudYonkes.BeginTransactionAsync(cancellationToken);

			try
			{
				// 1. Obtener la solicitud.
				var solicitud = await _unitOfWorkSolicitudes
					.SolicitudesRepository.GetByGuidId(solicitudGuidId);

				// 2. Validar estado
				if (solicitud.EstatusSolicitudId ==	(int)EstatusSolicitudEnum.Enviada)
				{
					throw new BusinessException("La solicitud ya fue enviada.");
				}

				// 3. Obtener las ciudades de búsqueda.
				var ciudadesSolicitud = await _unitOfWorkSolicitudes
					.SolicitudCiudadesRepository
					.ObtenerPorSolicitudAsync(solicitudGuidId);

				if (ciudadesSolicitud == null || !ciudadesSolicitud.Any())
				{
					throw new BusinessException("La solicitud no tiene ciudades asignadas.");
				}

				var ciudadesIds = ciudadesSolicitud
					.Select(x => x.CiudadId)
					.Distinct()
					.ToList();

				// 4. Obtener los yonkes con cobertura.
				var yonkes = await _unitOfWorkNegocio
					.YunkeCoberturaRepository
					.ObtenerPorCiudadesAsync(ciudadesIds);

				if (yonkes == null || !yonkes.Any())
				{
					throw new BusinessException("No existen yonkes con cobertura para las ciudades seleccionadas.");
				}

				// 5. Eliminar duplicados.
				var yonkesUnicos = yonkes
					.GroupBy(x => x.YonkeGuidId)
					.Select(x => x.First())
					.ToList();

				if (!yonkesUnicos.Any())
				{
					throw new BusinessException("No se encontraron yonkes disponibles.");
				}

				// 6. Validar si ya fue enviada a cada yonke.
				var yonkesNuevos = new List<YonkesCoberturas>();

				foreach (var yonkeCobertura in yonkesUnicos)
				{
					var existeEnvio = await _unitOfWorkSolicitudYonkes
						.SolicitudYonkeRepository
						.ExisteEnvioAsync(solicitud.GuidId, yonkeCobertura.YonkeGuidId);

					if (!existeEnvio)
					{
						yonkesNuevos.Add(yonkeCobertura);
					}
				}

				if (!yonkesNuevos.Any())
				{
					throw new BusinessException("La solicitud ya fue enviada a todos los yonkes disponibles.");
				}

				
				// 7. Crear la colección de SolicitudYonkes.
				var solicitudesYonkes = yonkesNuevos
					.Select(y => new SolicitudYonkes
					{
						GuidId = Guid.NewGuid(),
						SolicitudGuidId = solicitud.GuidId,
						YonkeGuidId = y.YonkeGuidId,
						EstatusId = (int)SolicitudYonkeEstatusEnum.Enviada,
						FechaEnvio = DateTime.UtcNow						
					}).ToList();

				// 8. Validar que existan envíos.
				if (solicitudesYonkes.Count == 0)
				{
					throw new BusinessException("No existen envíos para registrar.");
				}

				// Cambiar el estatus de la solicitud
				solicitud.EstatusSolicitudId = (int)EstatusSolicitudEnum.Enviada;

				// Guardar los envíos
				await _unitOfWorkSolicitudYonkes.SolicitudYonkeRepository.AgregarRangoAsync(solicitudesYonkes);


				//Historial de la solicitud
				var usuarioId = _currentUserService.UserId;

				if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
				{
					throw new BusinessException(
						"No fue posible identificar al usuario.");
				}

				var historial = new SolicitudesHistorials
				{
					SolicitudGuidId = solicitud.GuidId,
					EstatusSolicitudId = (int)EstatusSolicitudEnum.Enviada,
					Fecha = DateTime.UtcNow,
					UsuarioId = usuarioId.Value,
					Comentarios = $"Se envió la solicitud a {solicitudesYonkes.Count} yonke{(solicitudesYonkes.Count == 1 ? "" : "s")}."
				};

				await _unitOfWorkSolicitudes.solicitudHistorialRepository.AddHistorialAsync(historial);


				await _unitOfWorkSolicitudes.SaveChangesAsync();

				await _unitOfWorkSolicitudYonkes.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				// 10. Enviar notificaciones
				await EnviarNotificacionFirebaseAsync(
					solicitud,
					yonkesNuevos,
					cancellationToken);

				// 11. Regresar la cantidad de yonkes
				return solicitudesYonkes.Count;
			}
			catch
			{
				await transaction.RollbackAsync(CancellationToken.None);
				throw;
			}
		}

		private async Task EnviarNotificacionFirebaseAsync(Solicitudes solicitud, List<YonkesCoberturas> yonkes, CancellationToken cancellationToken)
		{
			try
			{
				var yonkesIds = yonkes
					.Select(x => x.YonkeGuidId)
					.ToList();

				var tokens =
					await _unitOfWorkNegocio
						.YunkeRepository
						.ObtenerTokensFirebaseAsync(yonkesIds);

				if (tokens == null || !tokens.Any())
				{
					_logger.LogWarning(
						"No existen tokens Firebase para solicitud {SolicitudGuidId}",
						solicitud.GuidId);

					return;
				}

				await _firebaseNotificationService.SendMulticastAsync(
					tokens, 
					FirebaseNotifications.Titles.NuevaSolicitud, 
					FirebaseNotifications.Messages.NuevaSolicitud,
					new Dictionary<string, string>
					{
					{
						"SolicitudGuidId",
						solicitud.GuidId.ToString()
					},
					{
						"Tipo",
						FirebaseNotifications.Types.NuevaSolicitud
					}
					},
					cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Error enviando Firebase solicitud {SolicitudGuidId}",
					solicitud.GuidId);
			}
		}



		//Marcar como vista una solicitud
		public async Task MarcarComoVistaAsync(Guid solicitudYonkeGuidId, CancellationToken cancellationToken)
		{
			//var yonkeGuidId = _currentUserService.YonkeGuidId;

		
			//if (!yonkeGuidId.HasValue)
			//	throw new UnauthorizedAccessException();

			var solicitudYonke = await _unitOfWorkSolicitudYonkes
				.SolicitudYonkeRepository
				.GetDataSolicitudyonkeByGuidId(solicitudYonkeGuidId);

			if (solicitudYonke.EstatusId != (int)SolicitudYonkeEstatusEnum.Enviada)
			{
				return;
			}

			if (solicitudYonke == null)
				throw new BusinessException("La solicitud no existe.");

			//if (solicitudYonke.YonkeGuidId != yonkeGuidId.Value)
			//	throw new BusinessException("No tiene permisos para acceder a esta solicitud.");

			// Si ya fue vista no hacemos nada
			if (solicitudYonke.FechaVista.HasValue)
				return;

			solicitudYonke.EstatusId = (int)SolicitudYonkeEstatusEnum.Vista;
			solicitudYonke.FechaVista = DateTime.UtcNow;
			solicitudYonke.FechaCambioEstatus = DateTime.UtcNow;

			await _unitOfWorkSolicitudYonkes.SolicitudYonkeRepository.ActualizarAsync(solicitudYonke);

			await _unitOfWorkSolicitudYonkes.SaveChangesAsync(cancellationToken);
		}


		//Contar solicitudes nuevas by yonke
		public async Task<int> ContarPendientesPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _unitOfWorkSolicitudYonkes
				.SolicitudYonkeRepository
				.ContarPendientesPorYonkeAsync(YonkeGuidId);
		}


		//Mas reciente solicitud by yonke
		public async Task<SolicitudYonke_List_DTO?> SolicitudRecienteByYonke(Guid YonkeGuidId)
		{
			return await _unitOfWorkSolicitudYonkes
				.SolicitudYonkeRepository
				.SolicitudMasRecienteByYonke(YonkeGuidId);
		}


		//Todas las solicitdes de cada yonke
		public async Task<List<SolicitudYonke_List_DTO>> ObtenerSolicitudesPorYonkeAsync(Guid YonkeGuidId, CancellationToken cancellationToken)
		{
			return await _unitOfWorkSolicitudYonkes
				.SolicitudYonkeRepository
				.ObtenerSolicitudesPorYonkeAsync(YonkeGuidId, cancellationToken);
		}

		public async Task<List<SolicitudYonkeDestinatarioDTO>> ObtenerDestinatariosPorSolicitudAsync(Guid solicitudGuidId, CancellationToken cancellationToken)
		{
			var envios = await _unitOfWorkSolicitudYonkes
				.SolicitudYonkeRepository
				.ObtenerPorSolicitudAsync(solicitudGuidId);

			return envios.Select(x => new SolicitudYonkeDestinatarioDTO
			{
				SolicitudYonkeGuidId = x.GuidId,
				YonkeGuidId = x.YonkeGuidId,
				NombreYonke = x.Yonkes?.Nombre ?? "Yonke",
				Logo = x.Yonkes?.LogoUrl,
				Telefono = x.Yonkes?.Telefono,
				EstatusId = x.EstatusId,
				Estatus = x.SolicitudYonkesEstatus?.EstatusSolicitud ?? "Enviada",
				FechaEnvio = x.FechaEnvio,
				Vista = x.FechaVista.HasValue,
				FechaVista = x.FechaVista
			}).ToList();
		}
	}
}
