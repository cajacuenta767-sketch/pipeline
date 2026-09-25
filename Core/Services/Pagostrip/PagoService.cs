using Core.Entitys;
using Core.Enums;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.PagosStripe;

namespace Core.Services.Pagostrip
{
	public class PagoService : IPagoService
	{
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes;

		public PagoService(IUnitOfWorkSolicitudYonkes unitOfWork)
		{
			_unitOfWorkSolicitudYonkes = unitOfWork;
		}


		// ==========================================
		// OBTENER POR GUID
		// ==========================================

		public async Task<Pagos?> ObtenerPorGuidAsync(Guid guidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (usuarioId == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No se pudo identificar al usuario.");
			}

			var pago =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorGuidAsync(
						guidId,
						cancellationToken);

			if (pago == null)
				return null;

			if (pago.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para consultar este pago.");
			}

			return pago;
		}


		// ==========================================
		// OBTENER POR ORDEN
		// ==========================================

		public async Task<Pagos?> ObtenerPorOrdenAsync(Guid ordenGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (usuarioId == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No se pudo identificar al usuario.");
			}

			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorGuidAsync(
						ordenGuidId,
						cancellationToken);

			if (orden == null)
				return null;

			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para consultar este pago.");
			}

			return await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.ObtenerPorOrdenGuidAsync(
					ordenGuidId,
					cancellationToken);
		}


		// ==========================================
		// CREAR PAGO DESDE ORDEN
		// ==========================================

		public async Task<Pagos> CrearDesdeOrdenAsync(Guid ordenGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{			
			if (usuarioId == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No se pudo identificar al usuario.");
			}


			// ======================================
			// 1. OBTENER ORDEN
			// ======================================

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


			// ======================================
			// 2. VALIDAR USUARIO
			// ======================================

			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para realizar este pago.");
			}


			// ======================================
			// 3. BUSCAR PAGO EXISTENTE
			// ======================================

			var pagoExistente =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorOrdenGuidAsync(
						orden.GuidId,
						cancellationToken);

			if (pagoExistente != null)
			{
				return pagoExistente;
			}


			// ======================================
			// 4. VALIDAR IMPORTE
			// ======================================

			if (orden.TotalCliente <= 0)
			{
				throw new InvalidOperationException(
					"La orden no tiene un importe válido.");
			}


			// ======================================
			// 5. CREAR PAGO
			// ======================================

			var pago = new Pagos
			{
				GuidId =
					Guid.NewGuid(),

				CotizacionGuidId =
					orden.CotizacionGuidId,

				OrdenGuidId =
					orden.GuidId,

				UsuarioId =
					usuarioId,

				Importe =
					orden.TotalCliente,

				ComisionRefanetPorcentaje =
					orden.ComisionRefanetPorcentaje,

				ComisionRefanetImporte =
					orden.ComisionRefanetImporte,

				ImporteYonke =
					orden.ImporteYonke,

				Moneda =
					"MXN",

				Estado =
					EstadoPagoCotizacion.Pendiente,

				FechaCreacion =
					DateTime.UtcNow
			};


			// ======================================
			// 6. GUARDAR
			// ======================================

			await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.AgregarAsync(
					pago,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);


			return pago;
		}


		// ==========================================
		// MARCAR PROCESANDO
		// ==========================================

		public async Task MarcarProcesandoAsync(Guid pagoGuidId, CancellationToken cancellationToken = default)
		{
			var pago =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);

			if (pago == null)
			{
				throw new KeyNotFoundException(
					"El pago no existe.");
			}

			pago.Estado =
				EstadoPagoCotizacion.Procesando;


			await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);
		}


		// ==========================================
		// MARCAR PAGADO
		// ==========================================

		public async Task MarcarPagadoAsync(Guid pagoGuidId, string? paymentIntentId = null, string? checkoutSessionId = null,  CancellationToken cancellationToken = default)
		{
			var pago =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);

			if (pago == null)
			{
				throw new KeyNotFoundException(
					"El pago no existe.");
			}


			// ======================================
			// IDEMPOTENCIA
			// ======================================

			if (pago.Estado ==
				EstadoPagoCotizacion.Pagado)
			{
				return;
			}


			pago.Estado =
				EstadoPagoCotizacion.Pagado;

			pago.FechaPago =
				DateTime.UtcNow;


			if (!string.IsNullOrWhiteSpace(
				paymentIntentId))
			{
				pago.StripePaymentIntentId =
					paymentIntentId;
			}


			if (!string.IsNullOrWhiteSpace(
				checkoutSessionId))
			{
				pago.StripeCheckoutSessionId =
					checkoutSessionId;
			}


			pago.StripePaymentStatus =
				"paid";


			await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);


			// ======================================
			// ACTUALIZAR ORDEN
			// ======================================

			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorGuidAsync(
						pago.OrdenGuidId,
						cancellationToken);

			if (orden != null)
			{
				// Pagada
				orden.EstatusOrdenId =
					3;

				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ActualizarAsync(
						orden,
						cancellationToken);
			}


			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);
		}


		// ==========================================
		// MARCAR FALLIDO
		// ==========================================

		public async Task MarcarFallidoAsync(Guid pagoGuidId, string? error = null, CancellationToken cancellationToken = default)
		{
			var pago =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);

			if (pago == null)
			{
				throw new KeyNotFoundException(
					"El pago no existe.");
			}


			pago.Estado =
				EstadoPagoCotizacion.Fallido;

			pago.StripePaymentStatus =
				"failed";

			pago.Error =
				error;


			await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);

			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);
		}


		// ==========================================
		// MARCAR REEMBOLSADO
		// ==========================================

		public async Task MarcarReembolsadoAsync(Guid pagoGuidId, CancellationToken cancellationToken = default)
		{
			var pago =
				await _unitOfWorkSolicitudYonkes
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);

			if (pago == null)
			{
				throw new KeyNotFoundException(
					"El pago no existe.");
			}


			if (pago.Estado ==
				EstadoPagoCotizacion.Reembolsado)
			{
				return;
			}


			pago.Estado =
				EstadoPagoCotizacion.Reembolsado;

			pago.StripePaymentStatus =
				"refunded";

			pago.FechaReembolso =
				DateTime.UtcNow;


			await _unitOfWorkSolicitudYonkes
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);


			// ======================================
			// ACTUALIZAR ORDEN
			// ======================================

			var orden =
				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ObtenerPorGuidAsync(
						pago.OrdenGuidId,
						cancellationToken);

			if (orden != null)
			{
				// Reembolsada
				orden.EstatusOrdenId =
					10;

				await _unitOfWorkSolicitudYonkes
					.OrdenRepository
					.ActualizarAsync(
						orden,
						cancellationToken);
			}


			await _unitOfWorkSolicitudYonkes
				.SaveChangesAsync(
					cancellationToken);
		}
	}





}
