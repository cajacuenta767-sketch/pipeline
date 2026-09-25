using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
	public class TipoArchivoLogotipo : ValidationAttribute
	{
		private readonly string[] tiposValidos;
		public TipoArchivoLogotipo(string[] tiposValidos)
		{
			this.tiposValidos = tiposValidos;
		}

		public TipoArchivoLogotipo(GrupoArchivos grupoArchivo)
		{
			if (grupoArchivo == GrupoArchivos.Logotipo)
			{
				tiposValidos = new string[] { "image/jpg", "image/jpeg", "image/png", "application/pdf" };
			}
		}


		protected override ValidationResult IsValid(object value, ValidationContext validationContext)

		{
			if (value == null)
			{

				return ValidationResult.Success;

			}


			IFormFile formfile = value as IFormFile;


			if (formfile == null)
			{

				return ValidationResult.Success;

			}

			if (!tiposValidos.Contains(formfile.ContentType))
			{
				return new ValidationResult($"El tipo de archivo debe ser {string.Join(", ", tiposValidos)}");
			}


			return ValidationResult.Success;

		}
	}
}
