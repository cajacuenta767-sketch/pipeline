using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class YonkesDispositivos : BaseEntity
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid YonkeGuidId { get; set; }
		public string? FirebaseToken { get; set; }
		public string Plataforma { get; set; }
		public string Modelo { get; set; }
		public bool Activo { get; set; }
		public DateTime FechaRegistro { get; set; }
		public DateTime? UltimoAcceso { get; set; }


		[ForeignKey(nameof(YonkeGuidId))]
		public virtual Yonkes Yonkes { get; set; }
	}
}
