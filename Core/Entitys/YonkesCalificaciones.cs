using Core.EntityBase;

namespace Core.Entitys
{
	public class YonkesCalificaciones : BaseEntity
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid YonkeGuidId { get; set; }
		public Guid UsuarioId { get; set; }
		public Guid SolicitudGuidId { get; set; }
		public Guid CotizacionGuidId { get; set; }
		public int Calificacion { get; set; }
		public string? Comentario { get; set; }
		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
		public bool Activa { get; set; } = true;

		public virtual Yonkes Yonkes { get; set; }
		public virtual Solicitudes Solicitudes { get; set; }
		public virtual SolicitudCotizaciones Cotizacion { get; set; }
	}
}
