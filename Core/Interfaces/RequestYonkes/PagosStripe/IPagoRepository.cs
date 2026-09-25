using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.PagosStripe
{
	namespace Core.Interfaces.Repositories
	{
		public interface IPagoRepository
		{
			Task<Pagos?> ObtenerPorGuidAsync(
				Guid guidId,
				CancellationToken cancellationToken = default);

			Task<Pagos?> ObtenerPorOrdenGuidAsync(
				Guid ordenGuidId,
				CancellationToken cancellationToken = default);

			Task<Pagos?> ObtenerPorCotizacionGuidAsync(
				Guid cotizacionGuidId,
				CancellationToken cancellationToken = default);

			Task<Pagos?> ObtenerPorStripePaymentIntentAsync(
				string paymentIntentId,
				CancellationToken cancellationToken = default);

			Task<Pagos?> ObtenerPorStripeCheckoutSessionAsync(
				string checkoutSessionId,
				CancellationToken cancellationToken = default);

			Task<Pagos?> ObtenerPorStripeEventIdAsync(
				string stripeEventId,
				CancellationToken cancellationToken = default);

			Task AgregarAsync(
				Pagos pago,
				CancellationToken cancellationToken = default);

			Task ActualizarAsync(
				Pagos pago,
				CancellationToken cancellationToken = default);

			Task<bool> ExistePagoPorOrdenAsync(
				Guid ordenGuidId,
				CancellationToken cancellationToken = default);
		}
	}
}
