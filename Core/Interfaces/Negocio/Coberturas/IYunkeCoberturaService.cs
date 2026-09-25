using Core.DTO.Empresas;
using Core.Entitys;

namespace Core.Interfaces.Negocio.Coberturas
{
	public interface IYunkeCoberturaService
	{
		/// <summary>
		/// Obtiene las ciudades de cobertura de un yonke.
		/// </summary>
		Task<IList<YonkesCoberturas>> ObtenerPorYonkeAsync(Guid YonkeGuidId);

		/// <summary>
		/// Obtiene las ciudades de cobertura por Guid del yonke.
		/// </summary>
		//Task<IList<YunkeWithCoberturasDTO>> ObtenerPorYonkeGuidAsync(Guid yonkeGuidId);
		Task<YunkeWithCoberturasDTO?> ObtenerPorYonkeGuidAsync(Guid YonkeGuidId);

		/// <summary>
		/// Obtiene una cobertura específica.
		/// </summary>
		Task<YonkesCoberturas?> ObtenerAsync(Guid YonkeGuidId, int ciudadId);

		/// <summary>
		/// Verifica si una ciudad pertenece a la cobertura del yonke.
		/// </summary>
		Task<bool> ExisteAsync(Guid YonkeGuidId, int ciudadId);

		/// <summary>
		/// Actualiza las ciudades de cobertura del yonke.
		/// Agrega las nuevas y elimina las que ya no estén seleccionadas.
		/// </summary>
		Task ActualizarCoberturasAsync(Guid YonkeGuidId, IList<int> ciudadesIds);


	

		


	}
}
