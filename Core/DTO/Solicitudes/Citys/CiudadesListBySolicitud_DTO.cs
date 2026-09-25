namespace Core.DTO.Solicitudes.Citys
{
	public class CiudadesListBySolicitud_DTO
	{
		public SolicitudHeader SolicitudHeader { get; set; } = new();
	}

	public class SolicitudHeader
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; }

		public string Folio { get; set; } = string.Empty;

		public IList<CiudadesSaveBySolicitud> CiudadesSaveBySolicitud { get; set; }
			= new List<CiudadesSaveBySolicitud>();
	}

	public class CiudadesSaveBySolicitud
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; }

		public int CiudadId { get; set; }

		public string Ciudad { get; set; } = string.Empty;
	}
}
