namespace Core.DTO.Login.otp
{
	public class ClienteInfoResponse
	{
		public string Id { get; set; } = null!;
		public string Telefono { get; set; } = null!;
		public string TipoUsuario { get; set; } = "Cliente";
		public bool TelefonoConfirmado { get; set; }
	}
}
