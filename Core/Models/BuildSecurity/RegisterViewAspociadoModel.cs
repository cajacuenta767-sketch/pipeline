using System.ComponentModel.DataAnnotations;

namespace Core.Models.BuildSecurity
{
	public class RegisterViewAspociadoModel
	{
		[Required]
		[StringLength(50)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string Password { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string ConfirmPassword { get; set; }

		public string PhoneNumber { get; set; }
		public bool	PhoneNumberConfirmed { get; set; }

		[Required]
		public string Role { get; set; }

		[Required]
		public int EmpresaId { get; set; }
	}
}
