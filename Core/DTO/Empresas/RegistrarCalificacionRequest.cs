namespace Core.DTO.Empresas
{
	public class RegistrarCalificacionRequest
	{
		public Guid CotizacionGuidId { get; set; }
		public int Calificacion { get; set; }
		public string? Comentario { get; set; }
	}
}
