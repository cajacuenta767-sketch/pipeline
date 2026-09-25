using System.ComponentModel.DataAnnotations;

namespace Core.Models.BuildSecurity
{
	public class RegisterViewSoporteModel
	{
		[Required]
		[StringLength(50)]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string? Password { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string? ConfirmPassword { get; set; }

	}
}
