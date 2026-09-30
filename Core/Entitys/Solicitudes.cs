using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class Solicitudes : BaseEntity
	{
		public Solicitudes()
		{
			solicitudesImagenes = new HashSet<SolicitudesImagenes>();
			solicitudesHistorials = new HashSet<SolicitudesHistorials>();
			SolicitudesCiudades = new HashSet<SolicitudesCiudades>();

			SolicitudYonkes = new HashSet<SolicitudYonkes>();

			YonkesCalificaciones = new HashSet<YonkesCalificaciones>();
		}

		//[NotMapped]
		public int Id { get; set; }
		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
		public Guid GuidId { get; set; }
		public Guid UsuarioId { get; set; }
		public int EstatusSolicitudId { get; set; }	
		public int MarcaId { get; set; }
		public int ModeloId { get; set; }
		public int Año { get; set; }
		public string Motor { get; set; }
		public string? Transmicion { get; set; }
		public string PiezaBuscada { get; set; }
		public string? NumeroParte { get; set; }
		public string Descripcion { get; set; }


		public string Folio { get; set; }
		public DateTime? FechaCierre { get; set; }		
		public bool Cerrada { get; set; }




		[ForeignKey("EstatusSolicitudId")]
		public virtual SolicitudesEstatus SolicitudEstatus { get; set; }

		[ForeignKey("MarcaId")]
		public virtual Marcas Marcas { get; set; }

		[ForeignKey("ModeloId")]
		public virtual Modelos Modelos { get; set; }


		public ICollection<SolicitudesImagenes> solicitudesImagenes { get; set; }
		public ICollection<SolicitudesHistorials> solicitudesHistorials { get; set; }
		public ICollection<SolicitudYonkes> SolicitudYonkes { get; set; }
		public virtual ICollection<SolicitudesCiudades> SolicitudesCiudades { get; set; }= new List<SolicitudesCiudades>();


		public ICollection<YonkesCalificaciones> YonkesCalificaciones { get; set; }
	}
}
