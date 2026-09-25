using Core.EntityBase;
using Core.Entitys;

namespace Core.Interfaces.Negocio
{
	public interface IRepositorioYunkeCobertura<T> where T : BaseEntity
	{
		
		Task AgregarAsync(YonkesCoberturas yunkeCoberturaSave);

		Task AgregarRangoAsync(IList<YonkesCoberturas> entities);

		void Eliminar(YonkesCoberturas entity);

		//void EliminarRango(IList<YonkesCoberturas> entities);
		Task EliminarRangoAsync(IEnumerable<int> ids);
	}
}
