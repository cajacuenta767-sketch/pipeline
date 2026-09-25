namespace Core.DTO.Empresas
{
	public class YunkeUpdateInfoDTO
	{
		public string Nombre { get; set; }
		public string Responsable { get; set; }

		public string Telefono { get; set; }
		//public string Correo { get; set; }
		public string Direccion { get; set; }
		public int CP { get; set; }
		public int CiudadId { get; set; }

		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
	}
}
