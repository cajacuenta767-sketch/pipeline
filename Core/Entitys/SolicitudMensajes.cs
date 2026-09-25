using Core.EntityBase;

namespace Core.Entitys
{
	public class SolicitudMensajes : BaseEntity
	{
		public int Id { get; set; }
		public Guid	GuidId { get; set; } = Guid.NewGuid();
		public Guid SolicitudGuidId { get; set; }
		public string Mensaje { get; set; }
		public bool Leido { get; set; }
		public DateTime FechaLectura { get; set; }
		public DateTime FechaCreacion { get; set; }
	}
}
