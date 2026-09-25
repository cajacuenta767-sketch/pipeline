using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudesHistorials : BaseEntity
	{
		public int Id { get; set; }
		public Guid SolicitudGuidId { get; set; }
		public int EstatusSolicitudId { get; set; }
		public DateTime Fecha { get; set; }
		public Guid UsuarioId { get; set; }
		public string Comentarios { get; set; }


		public virtual Solicitudes Solicitudes { get; set; }

		[ForeignKey("EstatusSolicitudId")]
		public virtual SolicitudesEstatus SolicitudesEstatus { get; set; }
	}
}
