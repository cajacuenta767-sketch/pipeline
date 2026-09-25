using Core.DTO.Empresas;
using Core.Entitys;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Negocio.Empresa
{
    public interface IYonkeservice
    {
		Task<IQueryable<YonkesListDTO>> getYonkesByEntidad(int? ciudadId);
		Task<YonkesListDTO?> getYunkeByGuidId(Guid guidId);
		Task<Yonkes?> getYonkeGuidId(Guid guidId);
		Task<Yonkes?> getYunkeById(int Id);
		Task<YonkesaveDTO> NuevoYunkeAsync(YonkesaveDTO model, CancellationToken cancellationToken = default);
		Task<bool> UpdateYunke(Yonkes Yonkes);
		Task<bool> BajaYunke(Guid GuidId);


		//Nuevo método
		Task<bool> ActualizarLogoAsync(Guid yonkeGuidId, IFormFile logo);
	}
}
