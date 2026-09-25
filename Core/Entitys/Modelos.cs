using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class Modelos : BaseEntity
	{
		public Modelos()
		{
			solicitudes = new HashSet<Solicitudes>();
		}

		public int Id { get; set; }
		public int MarcaId { get; set; }
		public string Modelo { get; set; }

		[ForeignKey("MarcaId")]
		public virtual Marcas Marcas { get; set; }


		public ICollection<Solicitudes> solicitudes { get; set; }
	}
}
