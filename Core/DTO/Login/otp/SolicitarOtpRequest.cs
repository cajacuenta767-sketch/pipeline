using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Login.otp
{
	public class SolicitarOtpRequest
	{
		[Required]
		[Phone]
		public string Telefono { get; set; } = null!;
	}
}
