namespace Core.DTO.Solicitudes.Requests
{
	public class Solicitudes_Create_DTO
	{
		public int MarcaId { get; set; }

		public int ModeloId { get; set; }

		public int Año { get; set; }

		public string Motor { get; set; }

		public string Transmicion { get; set; }

		public string PiezaBuscada { get; set; }

		public string NumeroParte { get; set; }

		public string Descripcion { get; set; }

		public IList<int> CiudadesIds { get; set; } = new List<int>();
	}
}
