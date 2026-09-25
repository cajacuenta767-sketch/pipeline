using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
	public class PesoArchivoLogotipo : ValidationAttribute
	{
		private readonly int pesoMaximoEnMegaBytes;
		public PesoArchivoLogotipo(int PesoMaximoEnMegaBytes)
		{
			pesoMaximoEnMegaBytes = PesoMaximoEnMegaBytes;
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

			if (formfile.Length > pesoMaximoEnMegaBytes * 1024 * 1024)
			{
				return new ValidationResult($"El peso maximo del archivo no debe ser mayor a {pesoMaximoEnMegaBytes} mb");
			}


			return ValidationResult.Success;

		}
	}
}
