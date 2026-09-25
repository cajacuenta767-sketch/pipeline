namespace Core.DTO.Stripe
{
	public class StripeCheckoutResponse
	{
		public string SessionId { get; set; } = null!;

		public string CheckoutUrl { get; set; } = null!;
	}
}
