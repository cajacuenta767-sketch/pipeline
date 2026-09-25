using Core.DTO.Empresas;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.Calificacion;
using Core.Interfaces.RequestYonkes;

namespace Core.Services.Negocio.Empresa
{
	public class YunkeCalificacionService : IYunkeCalificacionService
	{
		private readonly IUnitOfWorkNegocio _unitOfWorkNegocio;
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes;

		private readonly ICurrentUserService _currentUserService;

		public YunkeCalificacionService(IUnitOfWorkNegocio unitOfWorkNegocio,
										IUnitOfWorkSolicitudYonkes unitOfWorkSolicitudYonkes,
										ICurrentUserService currentUserService)
		{
			_unitOfWorkNegocio = unitOfWorkNegocio;
			_unitOfWorkSolicitudYonkes = unitOfWorkSolicitudYonkes;
			_currentUserService = currentUserService;
		}

		public async Task<Guid> RegistrarAsync(RegistrarCalificacionRequest request, CancellationToken cancellationToken)
		{
			// ==========================================
			// 1. Validaciones básicas
			// ==========================================

			if (request == null)
			{
				throw new BusinessException(
					"La información de la calificación es requerida.");
			}

			if (request.CotizacionGuidId == Guid.Empty)
			{
				throw new BusinessException(
					"La cotización no es válida.");
			}

			if (request.Calificacion < 1 ||
				request.Calificacion > 5)
			{
				throw new BusinessException(
					"La calificación debe estar entre 1 y 5 estrellas.");
			}

			// ==========================================
			// 2. Usuario actual
			// ==========================================

			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			// ==========================================
			// 3. Obtener cotización
			// ==========================================

			var cotizacion =
				await _unitOfWorkSolicitudYonkes
					.SolicitudCotizacionRepository
					.ObtenerPorGuidAsync(
						request.CotizacionGuidId, cancellationToken);

			if (cotizacion == null)
			{
				throw new BusinessException(
					"La cotización no existe.");
			}

			// ==========================================
			// 4. Validar que exista solicitud
			// ==========================================

			if (cotizacion.SolicitudYonkes == null)
			{
				throw new BusinessException(
					"No fue posible obtener la solicitud del yonke.");
			}

			if (cotizacion.SolicitudYonkes.Solicitudes == null)
			{
				throw new BusinessException(
					"No fue posible obtener la solicitud original.");
			}

			// ==========================================
			// 5. Validar propietario de la solicitud
			// ==========================================

			var solicitud =
				cotizacion.SolicitudYonkes.Solicitudes;

			if (solicitud.UsuarioId != usuarioId)
			{
				throw new BusinessException(
					"No tienes permiso para calificar esta cotización.");
			}

			// ==========================================
			// 6. Validar estado
			// ==========================================

			if (cotizacion.SolicitudYonkes.EstatusId !=
				(int)SolicitudYonkeEstatusEnum.Recibida)
			{
				throw new BusinessException(
					"Solo puedes calificar al yonke después de recibir la pieza.");
			}

			// ==========================================
			// 7. Validar que no haya calificado antes
			// ==========================================

			var existe =
				await _unitOfWorkNegocio
					.YunkeCalificacionRepository
					.ExisteCalificacionAsync(
						request.CotizacionGuidId,
						usuarioId,
						cancellationToken);

			if (existe)
			{
				throw new BusinessException("Ya calificaste esta cotización.");
			}

			// ==========================================
			// 8. Obtener Yonke
			// ==========================================

			var yonkeGuidId = cotizacion.SolicitudYonkes.YonkeGuidId;

			// ==========================================
			// 9. Crear calificación
			// ==========================================

			var calificacion = new YonkesCalificaciones
			{
				GuidId = Guid.NewGuid(),
				YonkeGuidId = yonkeGuidId,
				UsuarioId = (Guid)usuarioId,
				SolicitudGuidId = solicitud.GuidId,
				CotizacionGuidId = cotizacion.GuidId,
				Calificacion = request.Calificacion,
				Comentario = string.IsNullOrWhiteSpace(request.Comentario)
							? null
							: request.Comentario.Trim(),
				FechaCreacion = DateTime.UtcNow,
				Activa = true
			};

			// ==========================================
			// 10. Guardar
			// ==========================================

			await _unitOfWorkNegocio
				.YunkeCalificacionRepository
				.AgregarAsync(
					calificacion,
					cancellationToken);

			await _unitOfWorkNegocio.SaveChangesAsync(cancellationToken);

			// ==========================================
			// 11. Regresar Guid
			// ==========================================

			return calificacion.GuidId;
		}

		public async Task<CalificacionYonkeDto> ObtenerCalificacionAsync(Guid yonkeGuidId, CancellationToken cancellationToken)
		{
			if (yonkeGuidId == Guid.Empty)
			{
				throw new BusinessException(
					"El identificador del yonke no es válido.");
			}

			var promedio =
				await _unitOfWorkNegocio
					.YunkeCalificacionRepository
					.ObtenerPromedioAsync(
						yonkeGuidId,
						cancellationToken);

			var total =
				await _unitOfWorkNegocio
					.YunkeCalificacionRepository
					.ObtenerTotalAsync(
						yonkeGuidId,
						cancellationToken);

			return new CalificacionYonkeDto
			{
				YonkeGuidId = yonkeGuidId,
				Promedio = promedio,
				TotalCalificaciones = total
			};
		}
	}
}
