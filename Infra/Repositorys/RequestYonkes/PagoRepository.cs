using Core.Entitys;
using Core.Interfaces.RequestYonkes;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.RequestYonkes
{
	public class PagoRepository : IPagosRepository
	{
		private readonly AplicationDBContext _context;

		private DbSet<Pagos> _entities => _context.Set<Pagos>();


		public PagoRepository(AplicationDBContext context)
		{
			_context = context;
		}


		// ==========================================
		// OBTENER POR GUID
		// ==========================================

		public async Task<Pagos?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.GuidId == guidId,
					cancellationToken);
		}


		// ==========================================
		// OBTENER POR ORDEN
		// ==========================================

		public async Task<Pagos?> ObtenerPorOrdenGuidAsync(Guid ordenGuidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.OrdenGuidId == ordenGuidId,
					cancellationToken);
		}


		// ==========================================
		// OBTENER POR COTIZACIÓN
		// ==========================================

		public async Task<Pagos?> ObtenerPorCotizacionGuidAsync(Guid cotizacionGuidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.CotizacionGuidId == cotizacionGuidId,
					cancellationToken);
		}


		// ==========================================
		// STRIPE PAYMENT INTENT
		// ==========================================

		public async Task<Pagos?> ObtenerPorStripePaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(paymentIntentId))
				return null;

			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x =>
						x.StripePaymentIntentId ==
						paymentIntentId,
					cancellationToken);
		}


		// ==========================================
		// STRIPE CHECKOUT SESSION
		// ==========================================

		public async Task<Pagos?> ObtenerPorStripeCheckoutSessionAsync(string checkoutSessionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(checkoutSessionId))
				return null;

			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x =>
						x.StripeCheckoutSessionId ==
						checkoutSessionId,
					cancellationToken);
		}


		// ==========================================
		// STRIPE EVENT ID
		// ==========================================

		public async Task<Pagos?> ObtenerPorStripeEventIdAsync(string stripeEventId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(stripeEventId))
				return null;

			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x =>
						x.StripeEventId ==
						stripeEventId,
					cancellationToken);
		}


		// ==========================================
		// AGREGAR
		// ==========================================

		public async Task AgregarAsync(Pagos pago, CancellationToken cancellationToken = default)
		{
			await _entities.AddAsync(
				pago,
				cancellationToken);
		}


		// ==========================================
		// ACTUALIZAR
		// ==========================================

		public Task ActualizarAsync(Pagos pago, CancellationToken cancellationToken = default)
		{
			_entities.Update(pago);

			return Task.CompletedTask;
		}


		// ==========================================
		// EXISTE PAGO
		// ==========================================

		public async Task<bool> ExistePagoPorOrdenAsync(Guid ordenGuidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.AnyAsync(
					x => x.OrdenGuidId == ordenGuidId,
					cancellationToken);
		}
	}

}
