using Core.EntityBase;

namespace Core.Interfaces.Requests.historials
{
	public interface ISolicitudHistorialRepository<T> where T : BaseEntity
	{
		Task AddHistorialAsync(T entity);
	}
}
