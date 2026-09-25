using System.ComponentModel.DataAnnotations;

namespace Core.Models.BuildSecurity
{
	public class ReenviarConfirmEmailViewModel
	{
		[Required]
		[StringLength(50)]
		[EmailAddress]
		public string? Email { get; set; }
	}
}
