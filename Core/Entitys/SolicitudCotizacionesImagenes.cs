using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudCotizacionesImagenes : BaseEntity
	{
		public int Id { get; set; }
		public DateTime CreateAt { get; set; } = DateTime.Now;
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid CotizacionGuidId { get; set; }
		public bool EsPrincipal { get; set; }
		public int Orden { get; set; }
		public string UrlImagen { get; set; } = string.Empty;
		public string RutaBlob { get; set; } = string.Empty;



		[ForeignKey(nameof(CotizacionGuidId))]
		public virtual SolicitudCotizaciones Cotizacion { get; set; } = null!;
	}
}
