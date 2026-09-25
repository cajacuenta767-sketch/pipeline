using Core.Validation;
using Microsoft.AspNetCore.Http;

namespace Core.DTO.Empresas
{
	public class YunkeUpdateLogoDTO
	{
		public Guid GuidId { get; set; }

		[PesoArchivoLogotipo(PesoMaximoEnMegaBytes: 4)]
		[TipoArchivoLogotipo(grupoArchivo: GrupoArchivos.Logotipo)]
		public IFormFile LogoUrl { get; set; }
	}
}
