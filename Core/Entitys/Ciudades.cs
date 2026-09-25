using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
    public class Ciudades: BaseEntity
    {
		public Ciudades()
		{
			Yonkes = new HashSet<Yonkes>();

			YonkesCoberturas = new HashSet<YonkesCoberturas>();

			SolicitudesCiudades = new HashSet<SolicitudesCiudades>();
		}

		public int Id { get; set; }
		public int EntidadId { get; set; }
		public string Ciudad { get; set; }

		[ForeignKey("EntidadId")]
		public virtual Entidades Entidades { get; set; }

		public virtual ICollection<Yonkes> Yonkes { get; set; }

		

		public virtual ICollection<SolicitudesCiudades> SolicitudesCiudades { get; set; }

		public virtual ICollection<YonkesCoberturas> YonkesCoberturas { get; set; }	= new HashSet<YonkesCoberturas>();
	}
}
