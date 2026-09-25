using Core.EntityBase;

namespace Core.Entitys
{
	public class Folios : BaseEntity
	{
		public int Id { get; set; }
		public string Tipo { get; set; } = string.Empty;
		public int Anio { get; set; }
		public int Consecutivo { get; set; }
	}
}
