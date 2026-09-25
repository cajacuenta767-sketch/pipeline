using Core.DTO.Empresas;
using Core.EntityBase;
using Core.Entitys;

namespace Core.Interfaces.Negocio
{
    public interface IRepositrioYunke<T> where T : BaseEntity
    {
		Task<IQueryable<YonkesListDTO>> getYonkesByCiudad(int? ciudadId);
		Task<YonkesListDTO?> getYunkeGuidById(Guid guidId);
		Task<Yonkes?> getGuidById(Guid guidId);
		Task<Yonkes?> getYunkeById(int id);
		Task addYunke(Yonkes YonkesaveDTO, CancellationToken cancellationToken = default);
		void UpdateYunke(Yonkes yunkeGrabarDTO);
		void BajaYunke(Yonkes yunkeBaja);
		Task<List<string>> ObtenerTokensFirebaseAsync(List<Guid> YonkeGuidId);
		Task<bool> ExisteAsync(string nombre, string correo, string telefono);
		Task<Yonkes> ObtenerPorCorreoAsync(string correo);


	}
}
