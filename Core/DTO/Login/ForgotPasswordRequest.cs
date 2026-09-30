using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Login
{
	public class ForgotPasswordRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
	}
}
