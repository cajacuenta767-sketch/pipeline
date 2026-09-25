using Core.DTO.Empresas;
using Core.Entitys;

namespace Core.Interfaces.Negocio.Coberturas
{
	public interface IYunkeCoberturaRepository
	{
		/// <summary>
		/// Agrega una cobertura.
		/// </summary>
		Task AgregarAsync(YonkesCoberturas yunkeCoberturaSave);

		/// <summary>
		/// Agrega varias coberturas.
		/// </summary>
		Task AgregarRangoAsync(IList<YonkesCoberturas> entities);

		/// <summary>
		/// Elimina una cobertura.
		/// </summary>
		Task EliminarAsync(YonkesCoberturas entity);

		/// <summary>
		/// Elimina varias coberturas.
		/// </summary>
		Task EliminarRangoAsync(IEnumerable<int> ids);

		/// <summary>
		/// Elimina todas las coberturas de un yonke.
		/// </summary>
		Task EliminarPorYonkeAsync(Guid YonkeGuidId);

		/// <summary>
		/// Verifica si una ciudad pertenece a la cobertura del yonke.
		/// </summary>
		Task<bool> ExisteAsync(Guid YonkeGuidId, int ciudadId);

		/// <summary>
		/// Obtiene una cobertura específica.
		/// </summary>
		Task<YonkesCoberturas?> ObtenerAsync(Guid YonkeGuidId, int ciudadId);

		/// <summary>
		/// Obtiene todas las coberturas de un yonke.
		/// </summary>
		Task<IList<YonkesCoberturas>> ObtenerPorYonkeAsync(Guid YonkeGuidId);

		/// <summary>
		/// Obtiene todas las coberturas de un yonke mediante su Guid.
		/// </summary>
		Task<YunkeWithCoberturasDTO?> ObtenerPorYonkeGuidAsync(Guid YonkeGuidId);


		Task<IList<YonkesCoberturas>> ObtenerPorCiudadesAsync(IList<int> ciudadesIds);





	}
}
