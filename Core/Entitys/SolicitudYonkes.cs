using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudYonkes : BaseEntity
	{
		public SolicitudYonkes()
		{
			SolicitudCotizaciones = new HashSet<SolicitudCotizaciones>();
		}
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid SolicitudGuidId { get; set; }
		public Guid YonkeGuidId { get; set; }
		public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
		public int EstatusId { get; set; }

		public DateTime? FechaVista { get; set; }
		public DateTime? FechaRespuesta { get; set; }
		public DateTime? FechaCambioEstatus { get; set; }




		public virtual Solicitudes Solicitudes { get; set; } = null!;

		[ForeignKey(nameof(YonkeGuidId))]
		public virtual Yonkes Yonkes { get; set; } = null!;


		[ForeignKey("EstatusId")]
		public virtual SolicitudYonkesEstatus SolicitudYonkesEstatus { get; set; }


		public ICollection<SolicitudCotizaciones> SolicitudCotizaciones { get; set; }

	}
}
