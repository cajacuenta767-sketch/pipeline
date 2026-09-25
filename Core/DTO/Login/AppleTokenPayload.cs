namespace Core.DTO.Login
{
	public class AppleTokenPayload
	{
		public string Subject { get; set; }

		public string Email { get; set; }

		public bool EmailVerified { get; set; }
	}
}
