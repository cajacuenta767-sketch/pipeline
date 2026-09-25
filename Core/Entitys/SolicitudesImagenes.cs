using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudesImagenes : BaseEntity
	{		
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid	 SolicitudGuidId { get; set; }
		public string UrlImagen { get; set; }
		public string RutaBlob { get; set; }
		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;


		
		public virtual Solicitudes Solicitudes { get; set; }


	}
}
