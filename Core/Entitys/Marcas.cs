using Core.EntityBase;

namespace Core.Entitys
{
	public class Marcas : BaseEntity
	{
		public Marcas()
		{
			solicitudes = new HashSet<Solicitudes>();
			modelos = new HashSet<Modelos>();	
		}
		public int Id { get; set; }
		public string Marca { get; set; }

		public ICollection<Solicitudes> solicitudes { get; set; }

		public ICollection<Modelos> modelos { get; set; }	
	}
}
