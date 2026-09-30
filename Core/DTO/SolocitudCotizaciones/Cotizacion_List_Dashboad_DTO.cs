namespace Core.DTO.SolocitudCotizaciones
{
	public class Cotizacion_List_Dashboad_DTO
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; } = Guid.NewGuid();		
		public Guid SolicitudYonkeGuidId { get; set; }
		public DateTime FechaCreacionCotizacion { get; set; }
		public Guid UsuarioId { get; set; }
		public string Folio { get; set; }
		public string PiezaBuscada { get; set; }
		public string Marca { get; set; }
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
		public int? EstadoPago { get; set; }
	}
}
