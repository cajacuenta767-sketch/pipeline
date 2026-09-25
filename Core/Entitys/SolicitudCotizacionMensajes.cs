using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudCotizacionMensajes : BaseEntity
	{
		public SolicitudCotizacionMensajes()
		{
		}

		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();		
		public Guid SolicitudCotizacionGuidId { get; set; }
		public Guid UsuarioId { get; set; } = Guid.Empty!;	
		/// Cliente = 1
		/// Yonke = 2
		public int TipoRemitenteId { get; set; }
		public string Mensaje { get; set; } = null!;
		public bool Leido { get; set; } = false;
		public DateTime? FechaLectura { get; set; }
		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;


		[ForeignKey(nameof(SolicitudCotizacionGuidId))]
		public virtual SolicitudCotizaciones SolicitudCotizacion { get; set; } = null!;
	}
}
