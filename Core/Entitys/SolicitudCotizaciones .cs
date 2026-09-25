using Core.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entitys
{
	public class SolicitudCotizaciones : BaseEntity
	{
		public SolicitudCotizaciones()
		{
			SolicitudCotizacionesImagenes = new HashSet<SolicitudCotizacionesImagenes>();
			SolicitudCotizacionMensajes = new HashSet<SolicitudCotizacionMensajes>();

			YonkesCalificaciones = new HashSet<YonkesCalificaciones>();
		}
		public int Id { get; set; }
		public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public Guid UsuarioId { get; set; }
		public Guid SolicitudYonkeGuidId { get; set; }
		public int MarcaId { get; set; }
		public int ModeloId { get; set; }
		public int Año { get; set; }

		public decimal Precio { get; set; }
		public bool Disponible { get; set; }
		public bool EsNueva { get; set; }		
		public string? NumeroParte { get; set; }

		public string? Comentarios { get; set; }
		public bool TieneGarantia { get; set; }
		public int DiasGarantia { get; set; }

		public bool EnvioDisponible { get; set; }
		public decimal? CostoEnvio { get; set; }
		public int? TiempoEntregaDias { get; set; }
		public bool Activo { get; set; }
		public int EstatusId { get; set; }




		[ForeignKey(nameof(SolicitudYonkeGuidId))]
		public virtual SolicitudYonkes SolicitudYonkes { get; set; } = null!;


		[ForeignKey(nameof(EstatusId))]
		public virtual SolicitudCotizacionEstatus SolicitudCotizacionEstatus { get; set; } = null!;


		public virtual ICollection<SolicitudCotizacionesImagenes> SolicitudCotizacionesImagenes { get; set; }
						= new List<SolicitudCotizacionesImagenes>();



		
		public virtual ICollection<SolicitudCotizacionMensajes> SolicitudCotizacionMensajes { get; set; }


		public ICollection<YonkesCalificaciones> YonkesCalificaciones { get; set; }

	}
}
