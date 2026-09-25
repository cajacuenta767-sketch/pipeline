using Core.EntityBase;

namespace Core.Entitys
{
	public class Ordens : BaseEntity
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();


		// ==========================
		// RELACIONES
		// ==========================

		public Guid CotizacionGuidId { get; set; }
		public Guid UsuarioId { get; set; } = Guid.Empty!;
		public Guid YonkeGuidId { get; set; }


		// ==========================
		// IMPORTES
		// ==========================

		public decimal PrecioPieza { get; set; }
		public decimal CostoEnvio { get; set; }
		public decimal TotalCliente { get; set; }
		public decimal BaseComision { get; set; }
		public decimal ComisionRefanetPorcentaje { get; set; }
		public decimal ComisionRefanetImporte { get; set; }
		public decimal ImporteYonke { get; set; }


		// ==========================
		// ESTADO
		// ==========================

		public int EstatusOrdenId { get; set; }


		// ==========================
		// FECHAS
		// ==========================

		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
		public DateTime? FechaAceptacion { get; set; }
		public DateTime? FechaCompletada { get; set; }
		public DateTime? FechaCancelacion { get; set; }
	}
}
