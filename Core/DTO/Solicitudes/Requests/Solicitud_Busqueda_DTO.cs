namespace Core.DTO.Solicitudes.Requests
{
	public class Solicitud_Busqueda_DTO
	{
		public int Id { get; set; }
		public DateTime FechaCreacion { get; set; }
		public Guid GuidId { get; set; }
		public int EstatusSolicitudId { get; set; }
		public string EstatusSolicitud { get; set; }
		public Guid UsuarioId { get; set; }
		public int MarcaId { get; set; }
		public string Marca { get; set; }
		public int ModeloId { get; set; }
		public string Modelo { get; set; }
		public int Año { get; set; }
		public string Motor { get; set; }
		public string Transmicion { get; set; }
		public string PiezaBuscada { get; set; }
		public string NumeroParte { get; set; }
		public string Descripcion { get; set; }


		public string Folio { get; set; }
		public DateTime? FechaCierre { get; set; }
		public int TotalCotizaciones { get; set; }
		public bool Cerrada { get; set; }
	}
}
