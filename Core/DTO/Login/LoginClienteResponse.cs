namespace Core.DTO.Google_Login
{
	public class LoginClienteResponse
	{
		public string Token { get; set; } = string.Empty;

		public Guid ClienteGuidId { get; set; }

		public string Nombre { get; set; } = string.Empty;

		public string Correo { get; set; } = string.Empty;

		public string FotoPerfil { get; set; } = string.Empty;
	}
}
