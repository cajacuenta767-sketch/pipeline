namespace Core.DTO.Empresas
{
    public class YunkeDTO
    {
		//public int Id { get; set; }
		public Guid GuidId { get; set; }
		public bool Autorizado { get; set; }
		public bool Estatus { get; set; }
		public string Nombre { get; set; }
		public string Responsable { get; set; }
		public string? LogoUrl { get; set; }
		public string Telefono { get; set; }
		public string Correo { get; set; }
		public string Direccion { get; set; }
		public int CP { get; set; }
		public int CiudadId { get; set; }

		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
		//public DateTime CreateAt { get; set; }
		//public string CreateBy { get; set; }
	}
}
