using Core.DTO.Stripe;
using Core.DTO.Stripe.Core.DTO.Stripe;

namespace Core.Interfaces.Stripe
{
	public interface IStripePaymentService
	{
		//Task<StripeCheckoutResponse> CrearCheckoutAsync(
		//   Guid cotizacionGuidId,
		//   string usuarioId,
		//   CancellationToken cancellationToken = default);

		//Task<bool> ProcesarWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken = default);


		Task<StripeCheckoutResponse> CrearCheckoutAsync(
		  Guid ordenGuidId,
		  Guid usuarioId,
		  CancellationToken cancellationToken = default);

		Task<bool> ProcesarWebhookAsync(
			string json,
			string signature,
			CancellationToken cancellationToken = default);



		Task<PagoResultadoResponse> ObtenerResultadoPagoAsync(
			string sessionId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);
	}
}
