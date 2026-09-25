namespace Core.DTO.SolicitudYonkes
{
	public class SolicitudYonkeDTO
	{
		public Guid SolicitudYonkeGuidId { get; set; }

		public Guid SolicitudGuidId { get; set; }

		public string Pieza { get; set; }

		public string Vehiculo { get; set; }

		public string Ciudad { get; set; }

		public DateTime FechaSolicitud { get; set; }

		public int EstatusId { get; set; }

		public List<string> Imagenes { get; set; } = new();
	}
}
