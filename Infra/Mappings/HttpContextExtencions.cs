using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infra.Mappings
{
	public static class HttpContextExtencions
	{
		public async static Task InsertarParametrosPaginados<T>(this HttpContext httpContext,
			IQueryable<T> queryable, int cantidadRegistrosPorPagina)
		{
			double cantidad = await queryable.CountAsync();
			double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
			httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString());
		}
	}
}
