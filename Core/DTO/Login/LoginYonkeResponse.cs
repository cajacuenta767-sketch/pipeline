namespace Core.DTO.Login
{
	public class LoginYonkeResponse
	{
		public string Token { get; set; } = string.Empty;

		public Guid YonkeGuidId { get; set; }

		public string Nombre { get; set; } = string.Empty;

		public string Correo { get; set; } = string.Empty;
	}
}
