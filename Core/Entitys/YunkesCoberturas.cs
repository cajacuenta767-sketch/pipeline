using Core.EntityBase;

namespace Core.Entitys
{
	public class YunkesCoberturas : BaseEntity
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; } = Guid.NewGuid();

		public Guid YunkeGuidId { get; set; }

		public int CiudadId { get; set; }

		public bool Activo { get; set; }

		public DateTime FechaRegistro { get; set; }


		public virtual Yonkes Yunkes { get; set; } = null!;
		public virtual Ciudades Ciudades { get; set; } = null!;
	}
}
