using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Login.otp
{
	public class VerificarOtpRequest
	{
		[Required]
		[Phone]
		public string Telefono { get; set; } = null!;

		[Required]
		[StringLength(6, MinimumLength = 6)]
		public string Codigo { get; set; } = null!;
	}
}
