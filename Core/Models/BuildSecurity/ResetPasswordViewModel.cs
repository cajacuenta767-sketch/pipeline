using System.ComponentModel.DataAnnotations;

namespace Core.Models.BuildSecurity
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Token { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 8)]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 8)]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
