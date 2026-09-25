using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Login.AuthSoporte
{
	public class CrearUsuarioSoporteDTO
	{
		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(100, MinimumLength = 2,
		ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
		public string Nombre { get; set; } = string.Empty;


		[Required(ErrorMessage = "El correo electrónico es obligatorio.")]
		[EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
		[StringLength(150,
	    ErrorMessage = "El correo electrónico no puede exceder los 150 caracteres.")]
		public string Email { get; set; } = string.Empty;


		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		[StringLength(100, MinimumLength = 8,
	    ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
		[RegularExpression(
		@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
		ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
		public string Password { get; set; } = string.Empty;

		[Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
		[StringLength(20,
	    ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
		public string? Telefono { get; set; }


		[Required(ErrorMessage = "El rol es obligatorio.")]
		[RegularExpression(
		"^(Soporte|Administrador|Asociado|Cliente)$",
		ErrorMessage = "El rol debe ser Soporte, Administrador, Asociado o Cliente.")]
		public string Role { get; set; } = string.Empty;
	}
}
