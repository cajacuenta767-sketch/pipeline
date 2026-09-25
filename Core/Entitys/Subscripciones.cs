using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
    public class Subscripciones : BaseEntity
    {
		//public int Id { get; set; }
		public int YunkeId { get; set; }
		public int PeriodoPagoId { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaTermino { get; set; }

		[ForeignKey("PeriodoPagoId")]
		public virtual SubcripcionPeriodos SubcripcionPeriodos { get; set; }

		[ForeignKey("EmpresaId")]
		public virtual Yonkes Yonkes { get; set; }
	}
}
