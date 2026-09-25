using Core.EntityBase;

namespace Core.Entitys
{
	public class YonkesCoberturas : BaseEntity
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; } = Guid.NewGuid();

		public Guid YonkeGuidId { get; set; }

		public int CiudadId { get; set; }

		public bool Activo { get; set; }

		public DateTime FechaRegistro { get; set; }


		public virtual Yonkes Yonkes { get; set; } = null!;
		public virtual Ciudades Ciudades { get; set; } = null!;
	}
}
