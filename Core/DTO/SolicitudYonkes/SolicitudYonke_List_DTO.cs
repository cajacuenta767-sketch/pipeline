namespace Core.DTO.SolicitudYonkes
{
	public class SolicitudYonke_List_DTO
	{
		public Guid SolicitudYonkeGuidId { get; set; }

		public Guid SolicitudGuidId { get; set; }

		public string? Folio { get; set; }

		public string? PiezaBuscada { get; set; }

		public string? NumeroParte { get; set; }

		public DateTime FechaSolicitud { get; set; }

		public DateTime FechaEnvio { get; set; }

		public int EstatusId { get; set; }

		public string? Estatus { get; set; }

		public bool Vista { get; set; }

		public DateTime? FechaVista { get; set; }

		public int? Cotizaciones { get; set; }

		public Guid ClienteGuidId { get; set; }

		public string? Descripcion { get; set; }

		public string? Marca { get; set; }

		public string? Modelo { get; set; }

		public int? Año { get; set; }

		public string? Motor { get; set; }

		public string? Transmicion { get; set; }

		public List<SolicitudImagen_DTO> Imagenes { get; set; } = new();

		public List<SolicitudCiudad_DTO> Ciudades { get; set; } = new();
	}
}
