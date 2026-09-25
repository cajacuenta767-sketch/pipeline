namespace Core.DTO.Solicitudes.imagnees
{
	public class SolicitudesImagenes_DTO
	{
		public Guid GuidId { get; set; }
		public Guid SolicitudGuidId { get; set; }
		public string UrlImagen { get; set; }
		public DateTime FechaCreacion { get; set; }
	}
}
