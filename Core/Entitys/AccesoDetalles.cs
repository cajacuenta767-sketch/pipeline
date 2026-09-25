using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class AccesoDetalles : BaseEntity
	{
		//public int Id { get; set; }
		public string UserId { get; set; }
		public int EmpresaId { get; set; }
		public bool Estatus { get; set; }

		[ForeignKey("EmpresaId")]
		public virtual Yonkes Yonkes { get; set; }
	}
}
