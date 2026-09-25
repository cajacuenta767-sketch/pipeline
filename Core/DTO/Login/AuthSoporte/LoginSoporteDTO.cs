using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Login.AuthSoporte
{
	public class LoginSoporteDTO
	{
		[Required(ErrorMessage = "El correo electrónico es obligatorio.")]
		[EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		public string Password { get; set; } = string.Empty;
	}
}
