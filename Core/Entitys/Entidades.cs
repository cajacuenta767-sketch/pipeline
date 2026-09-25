using Core.EntityBase;

namespace Core.Entitys
{
    public class Entidades: BaseEntity
    {
		public Entidades()
		{
			Ciudades = new HashSet<Ciudades>();
		}

		//public int Id { get; set; }
		public string Entidad { get; set; }


		public virtual ICollection<Ciudades> Ciudades { get; set; }
	}
}
