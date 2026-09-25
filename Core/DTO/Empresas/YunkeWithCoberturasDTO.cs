namespace Core.DTO.Empresas
{
	public class YunkeWithCoberturasDTO
	{
		public YunkeHeader YunkeHeader { get; set; }
	}

	public class YunkeHeader
	{
		public int Id { get; set; }
		public bool Estatus { get; set; }
		public string Nombre { get; set; }		
		public int  TotalCiudades { get; set; }

		public YunkeCoberturas[] YunkeCoberturas { get; set; }
	}

	public class YunkeCoberturas
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; }
		public Guid YonkeGuidId { get; set; }
		public int CiudadId { get; set; }
		public string Ciudad { get; set; }
		public bool Activo { get; set; }
		public DateTime FechaRegistro { get; set; }
	}

}
