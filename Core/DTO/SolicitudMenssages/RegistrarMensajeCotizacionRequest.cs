namespace Core.DTO.SolicitudMenssages
{
	public class RegistrarMensajeCotizacionRequest
	{
		public Guid SolicitudCotizacionGuidId { get; set; }

		public string Mensaje { get; set; } = null!;
	}
}
