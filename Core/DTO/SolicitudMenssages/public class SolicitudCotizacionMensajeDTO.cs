namespace Core.DTO.SolicitudMenssages
{
	public class SolicitudCotizacionMensajeDTO
	{
		public Guid GuidId { get; set; }

		public Guid SolicitudCotizacionGuidId { get; set; }

		public Guid UsuarioId { get; set; } = Guid.Empty!;

		public int TipoRemitenteId { get; set; }

		public string TipoRemitente { get; set; } = null!;

		public string Mensaje { get; set; } = null!;

		public bool Leido { get; set; }

		public DateTime? FechaLectura { get; set; }

		public DateTime FechaCreacion { get; set; }
	}
}
