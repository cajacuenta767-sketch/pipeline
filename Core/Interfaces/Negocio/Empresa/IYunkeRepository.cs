using Core.DTO.Empresas;
using Core.Entitys;

namespace Core.Interfaces.Negocio.Empresa
{
    public interface IYunkeRepository
    {
		Task<IQueryable<YonkesListDTO>> getYonkesByEntidad(int? ciudadId);
		Task<YonkesListDTO?> getYunkeGuidById(Guid guidId);
		Task<Yonkes> getYunkeById(int Id);
		Task NuevoYunke(Yonkes Yonkesave);
		Task<bool> UpdateYunke(Yonkes Yonkes);
		Task<bool> BajaYunke(Guid guidId);

		Task<IList<Yonkes>> ObtenerPorCiudadAsync(int ciudadId);


		Task<List<string>> ObtenerTokensFirebaseAsync(List<int> yonkesIds);
	}
}
