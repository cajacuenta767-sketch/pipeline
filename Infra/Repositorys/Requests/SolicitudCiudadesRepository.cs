using Core.DTO.Solicitudes.Citys;
using Core.Entitys;
using Core.Interfaces.Requests;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Requests
{
	public class SolicitudCiudadesRepository : ISolicitudCiudadesRepository
	{
		private readonly AplicationDBContext _context;

		private readonly DbSet<SolicitudesCiudades> _entities;


		public SolicitudCiudadesRepository(AplicationDBContext context)
		{
			_context = context;
			_entities = _context.Set<SolicitudesCiudades>();
		}

		public async Task<IList<SolicitudesCiudades>> ObtenerPorSolicitudAsync(Guid solicitudGuidId)
		{
			return await _entities
				   .AsNoTracking()
				   .Include(x => x.Ciudades)
				   .Where(x => x.SolicitudGuidId == solicitudGuidId)
				   .OrderBy(x => x.Ciudades.Ciudad)
				   .ToListAsync();
		}

	 	public async Task<CiudadesListBySolicitud_DTO?> ObtenerCiudadesPorSolicitudAsync(Guid solicitudGuidId)
		{
			return await _entities
		   .AsNoTracking()
		   .Include(x => x.Solicitudes)
		   .Include(x => x.Ciudades)
		   .Where(x => x.SolicitudGuidId == solicitudGuidId)
		   .GroupBy(x => new
		   {
			   x.Solicitudes.Id,
			   x.Solicitudes.GuidId,
			   x.Solicitudes.Folio
		   })
		   .Select(g => new CiudadesListBySolicitud_DTO
		   {
			   SolicitudHeader = new SolicitudHeader
			   {
				   Id = g.Key.Id,
				   GuidId = g.Key.GuidId,
				   Folio = g.Key.Folio,
				   CiudadesSaveBySolicitud = g
					   .OrderBy(x => x.Ciudades.Ciudad)
					   .Select(x => new CiudadesSaveBySolicitud
					   {
						   Id = x.Id,
						   GuidId = x.GuidId,
						   CiudadId = x.CiudadId,
						   Ciudad = x.Ciudades.Ciudad
					   })
					   .ToList()
			   }
		   }).FirstOrDefaultAsync();
		}


		
	


		public async Task AgregarAsync(SolicitudesCiudades entity)
		{
			await _entities.AddAsync(entity);
		}

		public async Task AgregarRangoAsync(IList<SolicitudesCiudades> entities)
		{
			await _entities.AddRangeAsync(entities);
		}

		public async Task<SolicitudesCiudades?> ObtenerAsync(Guid solicitudGuidId, int ciudadId)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(x =>
					x.SolicitudGuidId == solicitudGuidId &&
					x.CiudadId == ciudadId);
		}


		public async Task<bool> ExisteAsync(Guid solicitudGuidId, int ciudadId)
		{
			return await _entities
				.AnyAsync(x =>
					x.SolicitudGuidId == solicitudGuidId &&
					x.CiudadId == ciudadId);
		}


		public Task EliminarAsync(SolicitudesCiudades entity)
		{
			_entities.Remove(entity);

			return Task.CompletedTask;
		}


		public async Task EliminarPorSolicitudAsync(Guid solicitudGuidId)
		{
			var ciudades = await _entities
				.Where(x => x.SolicitudGuidId == solicitudGuidId)
				.ToListAsync();


			if (ciudades.Any())
			{
				_entities.RemoveRange(ciudades);
			}
		}


		public async Task ActualizarCiudadesAsync(Guid solicitudGuidId, IList<int> ciudadesIds)
		{
			var actuales = await _entities
				.Where(x => x.SolicitudGuidId == solicitudGuidId)
				.ToListAsync();

			_entities.RemoveRange(actuales);


			var nuevas = ciudadesIds.Select(x => new SolicitudesCiudades
			{
				GuidId = Guid.NewGuid(),
				SolicitudGuidId = solicitudGuidId,
				CiudadId = x
			}).ToList();


			await _entities.AddRangeAsync(nuevas);
		}

		
	}

		
}
