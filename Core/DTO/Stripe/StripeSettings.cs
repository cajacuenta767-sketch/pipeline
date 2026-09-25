namespace Core.DTO.Stripe
{
	public class StripeSettings
	{
		public string SecretKey { get; set; } = null!;
		public string PublishableKey { get; set; } = null!;
		public string WebhookSecret { get; set; } = null!;
		public string SuccessUrl { get; set; } = null!;
		public string CancelUrl { get; set; } = null!;

		public decimal ComisionRefanetPorcentaje { get; set; }
	}
}
