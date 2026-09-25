using System.ComponentModel.DataAnnotations;

namespace Core.Models.BuildSecurity
{
	public class RegistroClienteModel
	{
		[Required]
		[StringLength(50)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public bool PhoneNumberConfirmed { get; set; }

		//[HiddenInput(DisplayValue = false)]
		public string Password { get; set; }


		public string ConfirmPassword { get; set; }


		public string Role { get; set; }



	}
}
