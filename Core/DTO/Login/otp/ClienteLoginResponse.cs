namespace Core.DTO.Login.otp
{
	public class ClienteLoginResponse
	{
		public bool Success { get; set; }
		public string AccessToken { get; set; } = null!;
		public string? RefreshToken { get; set; }
		public ClienteInfoResponse Usuario { get; set; } = null!;
	}
}
