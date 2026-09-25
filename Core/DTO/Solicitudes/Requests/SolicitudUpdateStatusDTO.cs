namespace Core.DTO.Solicitudes.Requests
{
	public class SolicitudUpdateStatusDTO
	{
		public Guid GuidId { get; set; }
		public int? EstatusId { get; set; }
		public string UserId { get; set; }
		public string? Notas { get; set; }
	}
}
