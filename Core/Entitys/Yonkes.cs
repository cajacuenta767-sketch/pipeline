using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public partial class Yonkes : BaseEntity
	{
		public Yonkes()
		{
			YonkesCoberturas = new HashSet<YonkesCoberturas>();

			AccesoDetalles = new HashSet<AccesoDetalles>();
			Subscripciones = new HashSet<Subscripciones>();

			SolicitudYonkes = new HashSet<SolicitudYonkes>();

			YonkesDispositivos = new HashSet<YonkesDispositivos>();
		}

		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public bool Autorizado { get; set; }
		public bool Estatus { get; set; }
		public string Nombre { get; set; }
		public string Responsable { get; set; }
		public string? LogoUrl { get; set; }
		public string Telefono { get; set; }
		public string Correo { get; set; }
		public string Direccion { get; set; }
		public int CP { get; set; }
		public int CiudadId { get; set; }

		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
		public DateTime? CreateAt { get; set; }
		public Guid? CreateBy { get; set; }

		//Pagos
		public string? StripeAccountId { get; set; }
		public bool StripeAccountConectada { get; set; }
		public string? StripeAccountFechaConexion { get; set; }
		public string? StripeAccountStatus { get; set; }


		[ForeignKey("CiudadId")]
		public virtual Ciudades Ciudades { get; set; }


		public virtual ICollection<YonkesCoberturas> YonkesCoberturas { get; set; }

		public virtual ICollection<AccesoDetalles> AccesoDetalles { get; set; }

		public virtual ICollection<Subscripciones> Subscripciones { get; set; }

		public virtual ICollection<SolicitudYonkes> SolicitudYonkes { get; set; }

		public virtual ICollection<YonkesDispositivos> YonkesDispositivos { get; set; }

		public virtual ICollection<YonkesCalificaciones> YonkesCalificaciones { get; set; }
	}
}
