using Core.DTO.Empresas;
using Core.Entitys;
using Core.Interfaces.Negocio.Coberturas;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Negocio
{
	public class YunkeCoberturaRepository : IYunkeCoberturaRepository
	{
		//injectar la BD
		private readonly AplicationDBContext _context;	


		public YunkeCoberturaRepository(AplicationDBContext context)
		{
			_context = context;			
		}

		public async Task AgregarAsync(YonkesCoberturas yunkeCoberturaSave)
		{
			await _context.AddAsync(yunkeCoberturaSave);
		}

		public async Task AgregarRangoAsync(IList<YonkesCoberturas> entities)
		{
			await _context.AddRangeAsync(entities);
		}

		public Task EliminarAsync(YonkesCoberturas entity)
		{
			_context.Remove(entity);

			return Task.CompletedTask;
		}

		public async Task EliminarPorYonkeAsync(Guid YonkeGuidId)
		{
			var entidades = await _context.YonkesCoberturas
				.Where(x => x.YonkeGuidId == YonkeGuidId)
				.ToListAsync();

			if (entidades.Any())
			{
				_context.RemoveRange(entidades);
			}
		}


		//public Task EliminarRangoAsync(IEnumerable<int> ids)
		//{
		//	_context.RemoveRange(ids);
		//	return Task.CompletedTask;
		//}

		public async Task EliminarRangoAsync(IEnumerable<int> ids)
		{
			var entidades = await _context.YonkesCoberturas
				.Where(x => ids.Contains(x.Id))
				.ToListAsync();

			_context.YonkesCoberturas.RemoveRange(entidades);
		}

		public async Task<bool> ExisteAsync(Guid YonkeGuidId, int ciudadId)
		{
			return await _context.YonkesCoberturas
			  .AnyAsync(x =>
			  x.YonkeGuidId == YonkeGuidId &&
			  x.CiudadId == ciudadId);
		}

		public async Task<YonkesCoberturas?> ObtenerAsync(Guid YonkeGuidId, int ciudadId)
		{
			return await _context.YonkesCoberturas
			   .AsNoTracking()
			   .FirstOrDefaultAsync(x =>
				   x.YonkeGuidId == YonkeGuidId &&
				   x.CiudadId == ciudadId);
		}

		

		public  async Task<IList<YonkesCoberturas>> ObtenerPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _context.YonkesCoberturas
				.AsNoTracking()
				.Where(x => x.YonkeGuidId == YonkeGuidId)
				.OrderBy(x => x.CiudadId)
				.ToListAsync();
		}

		public async Task<YunkeWithCoberturasDTO?> ObtenerPorYonkeGuidAsync(Guid YonkeGuidId)
		{
			return await _context.YonkesCoberturas
				.AsNoTracking()
				.Include(x => x.Yonkes)
				.Where(x => x.Yonkes.GuidId == YonkeGuidId)
				.GroupBy(x => new
				{
					x.Yonkes.Id,
					x.Yonkes.Nombre,
					x.Yonkes.Estatus
				})
				.Select(g => new YunkeWithCoberturasDTO
				{
					YunkeHeader = new YunkeHeader
					{
						Id = g.Key.Id,
						Nombre = g.Key.Nombre,
						Estatus = g.Key.Estatus,
						TotalCiudades = g.Count(),
						YunkeCoberturas = g
							.OrderBy(c => c.CiudadId)
							.Select(c => new YunkeCoberturas
							{
								Id = c.Id,
								GuidId = c.GuidId,
								YonkeGuidId = c.YonkeGuidId,
								CiudadId = c.CiudadId,
								Ciudad = c.Ciudades.Ciudad,
								Activo = c.Activo,
								FechaRegistro = c.FechaRegistro
							}).ToArray()
					}
				}).FirstOrDefaultAsync();
		}


		public async Task<IList<YonkesCoberturas>> ObtenerPorCiudadesAsync(IList<int> ciudadesIds)
		{
			if (ciudadesIds == null || !ciudadesIds.Any())
				return new List<YonkesCoberturas>();

			return await _context.YonkesCoberturas
				.AsNoTracking()
				.Include(x => x.Yonkes)
				.Where(x => ciudadesIds.Contains(x.CiudadId))
				.OrderBy(x => x.YonkeGuidId)
				.ThenBy(x => x.CiudadId)
				.ToListAsync();
		}

		
	}
}
