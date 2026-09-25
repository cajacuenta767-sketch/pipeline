namespace Core.DTO.Stripe
{
	namespace Core.DTO.Stripe
	{
		public class PagoResultadoResponse
		{
			public bool Exitoso { get; set; }
			public string Estado { get; set; } = string.Empty;
			public string SessionId { get; set; } = string.Empty;
			public Guid OrdenGuidId { get; set; }
			public Guid PagoGuidId { get; set; }
			public decimal Importe { get; set; }
			public string Moneda { get; set; } = string.Empty;
			public string Mensaje { get; set; } = string.Empty;
		}
	}
}
