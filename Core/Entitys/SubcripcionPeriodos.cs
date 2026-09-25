using Core.EntityBase;

namespace Core.Entitys
{
    public class SubcripcionPeriodos: BaseEntity
    {
		public SubcripcionPeriodos()
		{
			Subscripciones = new HashSet<Subscripciones>();
		}
		//public int Id { get; set; }
		public string Periodo { get; set; }
		public decimal Precio { get; set; }


		public virtual ICollection<Subscripciones> Subscripciones { get; set; }
	}
}
