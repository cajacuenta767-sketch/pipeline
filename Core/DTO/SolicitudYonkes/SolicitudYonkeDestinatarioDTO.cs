namespace Core.DTO.SolicitudYonkes
{
	public class SolicitudYonkeDestinatarioDTO
	{
		public Guid SolicitudYonkeGuidId { get; set; }
		public Guid YonkeGuidId { get; set; }
		public string NombreYonke { get; set; } = string.Empty;
		public string? Logo { get; set; }
		public string? Telefono { get; set; }
		public int EstatusId { get; set; }
		public string? Estatus { get; set; }
		public DateTime FechaEnvio { get; set; }
		public bool Vista { get; set; }
		public DateTime? FechaVista { get; set; }
	}
}
