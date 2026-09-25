using Core.Pagination;

namespace Infra.Mappings
{
	public static class QueryableExtensions
	{
		public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
		{
			return queryable
				.Skip((paginacionDTO.Page - 1) * paginacionDTO.CantidadRegistrosPorPagina)
				.Take(paginacionDTO.CantidadRegistrosPorPagina);
		}
	}
}
