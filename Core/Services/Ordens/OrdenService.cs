using Core.Exceptions;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.OrdenesPago;

namespace Core.Services.Ordens
{
	public class OrdenService : IOrdenService
	{
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes;

		public OrdenService(IUnitOfWorkSolicitudYonkes unitOfWork)
		{
			_unitOfWorkSolicitudYonkes = unitOfWork;
		}

		// ==========================================
		// OBTENER ORDEN
		// ==========================================

		public async Task<Entitys.Ordens?> ObtenerPorGuidAsync(Guid guidId,	Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (usuarioId == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al cliente de la solicitud.");
			}

			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorGuidAsync(
						guidId,
						cancellationToken);

			if (orden == null)
				return null;

			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para consultar esta orden.");
			}

			return orden;
		}

		// ==========================================
		// OBTENER ORDEN POR COTIZACIÓN
		// ==========================================

		public async Task<Entitys.Ordens?> ObtenerPorCotizacionAsync(Guid cotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (usuarioId == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al cliente de la solicitud.");
			}

			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorCotizacionGuidAsync(
						cotizacionGuidId,
						cancellationToken);

			if (orden == null)
				return null;

			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para consultar esta orden.");
			}

			return orden;
		}


		// ==========================================
		// CREAR ORDEN DESDE COTIZACIÓN
		// ==========================================

		public async Task<Entitys.Ordens> CrearDesdeCotizacionAsync(Guid cotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (usuarioId == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al cliente de la solicitud.");
			}


			// ======================================
			// 1. OBTENER COTIZACIÓN
			// ======================================

			var cotizacion =
				await _unitOfWorkSolicitudYonkes
					.SolicitudCotizacionRepository
					.ObtenerPorGuidAsync(
						cotizacionGuidId,
						cancellationToken);

			if (cotizacion == null)
			{
				throw new KeyNotFoundException(
					"La cotización no existe.");
			}


			// ======================================
			// 2. VALIDAR PROPIETARIO
			// ======================================

			if (cotizacion.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para aceptar esta cotización.");
			}


			// ======================================
			// 3. VALIDAR COTIZACIÓN
			// ======================================

			if (!cotizacion.Activo)
			{
				throw new InvalidOperationException(
					"La cotización ya no está disponible.");
			}

			if (!cotizacion.Disponible)
			{
				throw new InvalidOperationException(
					"El yonke ya no tiene disponible la pieza.");
			}

			if (cotizacion.Precio <= 0)
			{
				throw new InvalidOperationException(
					"La cotización no tiene un precio válido.");
			}


			// ======================================
			// 4. VALIDAR QUE NO EXISTA ORDEN
			// ======================================

			var ordenExistente =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorCotizacionGuidAsync(
						cotizacionGuidId,
						cancellationToken);

			if (ordenExistente != null)
			{
				return ordenExistente;
			}


			// ======================================
			// 5. OBTENER SOLICITUD Y YONKE
			// ======================================

			var solicitudYonke =
				cotizacion.SolicitudYonkes;

			if (solicitudYonke == null)
			{
				throw new InvalidOperationException(
					"La cotización no tiene un yonke asociado.");
			}


			var yonkeGuidId = solicitudYonke.YonkeGuidId;


			// ======================================
			// 6. CALCULAR IMPORTES
			// ======================================

			var precioPieza = cotizacion.Precio;
			var costoEnvio = cotizacion.CostoEnvio ?? 0m;
			var totalCliente = precioPieza + costoEnvio;


			// ======================================
			// 7. COMISIÓN REFANET
			// ======================================

			const decimal comisionPorcentaje = 10m;

			var baseComision = 	precioPieza;
			var comisionRefanet =
				Math.Round(
					baseComision *
					(comisionPorcentaje / 100m),
					2,
					MidpointRounding.AwayFromZero);


			// ======================================
			// 8. IMPORTE YONKE
			// ======================================

			var importeYonke =
				totalCliente -
				comisionRefanet;


			// ======================================
			// 9. CREAR ORDEN
			// ======================================

			var orden = new Entitys.Ordens
			{
				GuidId = Guid.NewGuid(),
				CotizacionGuidId = cotizacion.GuidId,
				UsuarioId = usuarioId,
				YonkeGuidId = yonkeGuidId, 
				PrecioPieza = precioPieza,
				CostoEnvio = costoEnvio,
				TotalCliente = totalCliente,
				BaseComision = baseComision,
				ComisionRefanetPorcentaje = comisionPorcentaje,
				ComisionRefanetImporte = comisionRefanet,
				ImporteYonke = importeYonke,

				// 1 = PendientePago
				EstatusOrdenId =
					1,

				FechaCreacion =
					DateTime.UtcNow,

				FechaAceptacion =
					DateTime.UtcNow
			};


			// ======================================
			// 10. GUARDAR
			// ======================================

			await _unitOfWorkSolicitudYonkes
				.OrdenRepository
				.AgregarAsync(
					orden,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);


			return orden;
		}


		// ==========================================
		// ACTUALIZAR ESTATUS
		// ==========================================

		public async Task<Entitys.Ordens> ActualizarEstatusAsync(Guid ordenGuidId, Guid usuarioId, int estatusOrdenId, CancellationToken cancellationToken = default)
		{
			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorGuidAsync(
						ordenGuidId,
						cancellationToken);

			if (orden == null)
			{
				throw new KeyNotFoundException(
					"La orden no existe.");
			}


			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para modificar esta orden.");
			}


			orden.EstatusOrdenId =
				estatusOrdenId;


			// Ejemplo:
			// 7 = Completada

			if (estatusOrdenId == 7)
			{
				orden.FechaCompletada =
					DateTime.UtcNow;
			}


			// Ejemplo:
			// 9 = Cancelada

			if (estatusOrdenId == 9)
			{
				orden.FechaCancelacion =
					DateTime.UtcNow;
			}


			await _unitOfWorkSolicitudYonkes
				.OrdenRepository
				.ActualizarAsync(
					orden,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);


			return orden;
		}


	}
}
