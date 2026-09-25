using Core.EntityBase;

namespace Core.Entitys
{
	public class SolicitudesEstatus : BaseEntity
	{
		public SolicitudesEstatus()
		{
			solicitudes = new HashSet<Solicitudes>();
			solicitudesEstatuses = new HashSet<SolicitudesEstatus>();	
		}

		
		public int Id { get; set; }
		public string Estatus { get; set; }

		public ICollection<Solicitudes> solicitudes { get; set; }

		public ICollection<SolicitudesEstatus> solicitudesEstatuses { get; set; }
	}
}
