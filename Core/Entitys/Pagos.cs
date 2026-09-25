using Core.EntityBase;
using Core.Enums;

namespace Core.Entitys
{
	public class Pagos : BaseEntity
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid CotizacionGuidId { get; set; }
		public Guid OrdenGuidId { get; set; }
		public Guid UsuarioId { get; set; } = Guid.Empty!;


		// ==========================
		// IMPORTES
		// ==========================

		public decimal Importe { get; set; }
		public decimal ComisionRefanetPorcentaje { get; set; }
		public decimal ComisionRefanetImporte { get; set; }
		public decimal ImporteYonke { get; set; }
		public decimal? StripeFee { get; set; }
		public decimal ImporteStripe { get; set; }
		public decimal? GananciaRefanet { get; set; }


		// ==========================
		// MONEDA
		// ==========================

		public string Moneda { get; set; } = "MXN";


		// ==========================
		// ESTADO
		// ==========================

		public EstadoPagoCotizacion Estado { get; set; }
		public string? Error { get; set; }


		// ==========================
		// STRIPE
		// ==========================

		public string? StripeCheckoutSessionId { get; set; }
		public string? StripePaymentIntentId { get; set; }
		public string? StripeCustomerId { get; set; }
		public string? StripePaymentMethodId { get; set; }
		public string? StripeChargeId { get; set; }
		public string? StripeTransferId { get; set; }
		public string? StripePaymentStatus { get; set; }
		public string? StripeEventId { get; set; }
		public string? StripeAccountId { get; set; }


		// ==========================
		// FECHAS
		// ==========================

		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
		public DateTime? FechaPago { get; set; }
		public DateTime? FechaReembolso { get; set; }
	}
}
