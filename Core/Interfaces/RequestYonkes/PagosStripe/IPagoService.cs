using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.PagosStripe
{
	public interface IPagoService
	{
		Task<Pagos?> ObtenerPorGuidAsync(
			Guid guidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task<Pagos?> ObtenerPorOrdenAsync(
			Guid ordenGuidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task<Pagos> CrearDesdeOrdenAsync(
			Guid ordenGuidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task MarcarProcesandoAsync(
			Guid pagoGuidId,
			CancellationToken cancellationToken = default);

		Task MarcarPagadoAsync(
			Guid pagoGuidId,
			string? paymentIntentId = null,
			string? checkoutSessionId = null,
			CancellationToken cancellationToken = default);

		Task MarcarFallidoAsync(
			Guid pagoGuidId,
			string? error = null,
			CancellationToken cancellationToken = default);

		Task MarcarReembolsadoAsync(
			Guid pagoGuidId,
			CancellationToken cancellationToken = default);
	}
}
