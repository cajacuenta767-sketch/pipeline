namespace Core.DTO.Login
{
	public class AppleLoginRequest
	{
		public string IdentityToken { get; set; }

		public string AuthorizationCode { get; set; }

		// Apple puede enviarlo solamente en el primer login
		public string UserIdentifier { get; set; }

		public string Nombre { get; set; }

		public string Apellido { get; set; }
	}
}
