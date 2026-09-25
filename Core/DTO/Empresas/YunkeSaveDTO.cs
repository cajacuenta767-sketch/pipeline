using Core.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Empresas
{
    public class YonkesaveDTO
    {
		//public int Id { get; set; }
		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Responsable { get; set; }

		[Required]
		public string Telefono { get; set; }


		[Required(AllowEmptyStrings = false)]
		[EmailAddress]
		public string Correo { get; set; }

		[Required]
		public string Direccion { get; set; }

		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[0-9]+$", ErrorMessage = "Solo numeros")]
		public int CP { get; set; }

		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[0-9]+$", ErrorMessage = "Solo numeros")]
		public int CiudadId { get; set; }


		[PesoArchivoLogotipo(PesoMaximoEnMegaBytes: 2)]
		[TipoArchivoLogotipo(grupoArchivo: GrupoArchivos.Logotipo)]
		public IFormFile LogoUrl { get; set; }


		//Password por usuario definido
		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		[MinLength(8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
		public string Password { get; set; }


		[Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
		public string ConfirmPassword { get; set; }
	}
}
