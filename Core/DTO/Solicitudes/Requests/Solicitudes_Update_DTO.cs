namespace Core.DTO.Solicitudes.Requests
{
	public class Solicitudes_Update_DTO
	{
		public int MarcaId { get; set; }

		public int ModeloId { get; set; }

		public int Año { get; set; }

		public string Motor { get; set; }

		public string Transmision { get; set; }

		public string PiezaBuscada { get; set; }

		public string NumeroParte { get; set; }

		public string Descripcion { get; set; }
	}
}
