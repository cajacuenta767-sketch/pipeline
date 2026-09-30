using Core.DTO.SolicitudYonkes;
using Core.Entitys;
using Core.Enums;
using Core.Interfaces.RequestYonkes;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Infra.Repositorys.RequestYonkes
{
	public class SolicitudYonkeRepository : ISolicitudesYonkeRepository
	{
		private readonly AplicationDBContext _context;

		private readonly DbSet<SolicitudYonkes> _entities;

		public SolicitudYonkeRepository(AplicationDBContext context)
		{
			_context = context;
			_entities = _context.Set<SolicitudYonkes>();
		}

		public async Task AgregarAsync(SolicitudYonkes entity)
		{
			await _entities.AddAsync(entity);
		}

		public async Task AgregarRangoAsync(IList<SolicitudYonkes> entities)
		{
			await _entities.AddRangeAsync(entities);
		}

		public Task ActualizarAsync(SolicitudYonkes entity)
		{
			_entities.Update(entity);
			return Task.CompletedTask;
		}

		public Task EliminarAsync(SolicitudYonkes entity)
		{
			_entities.Remove(entity);
			return Task.CompletedTask;
		}

		public async Task<bool> ExisteEnvioAsync(Guid SolicitudGuidId, Guid YonkeGuidId)
		{
			return await _entities.AnyAsync(x =>
						x.SolicitudGuidId == SolicitudGuidId
						&& x.YonkeGuidId == YonkeGuidId);
		}

	

		public async Task<SolicitudYonkes?> ObtenerAsync(Guid SolicitudGuidId, Guid YonkeGuidId)
		{
			return await _entities
					   .AsNoTracking()
					   .Include(x => x.Solicitudes)
					   .Include(x => x.Yonkes)
					   .Include(x => x.SolicitudYonkesEstatus)
					   .Include(x => x.SolicitudCotizaciones)
					   .FirstOrDefaultAsync(x =>
						   x.SolicitudGuidId == SolicitudGuidId &&
						   x.YonkeGuidId == YonkeGuidId);
		}

		public async Task<SolicitudYonkes?> ObtenerPorGuidAsync(Guid guidId)
		{
			return await _entities
				      .AsNoTracking()
					  .Include(x => x.Solicitudes)
					  .Include(x => x.Yonkes)
					  .Include(x => x.SolicitudYonkesEstatus)
					  .Include(x => x.SolicitudCotizaciones)
					  .FirstOrDefaultAsync(x => x.GuidId == guidId);
		}


		
		public async Task<SolicitudYonkes?> GetDataSolicitudyonkeByGuidId(Guid guidId)
		{
			return await _entities					  
					  .Include(x => x.Solicitudes)
					  .Include(x => x.Yonkes)
					  .Include(x => x.SolicitudYonkesEstatus)
					  .Include(x => x.SolicitudCotizaciones)
					  .FirstOrDefaultAsync(x => x.GuidId == guidId);
		}

		public async Task<SolicitudYonkes?> ObtenerConSolicitudPorGuidAsync(Guid guidId)
		{
			return await _context.SolicitudYonkes
				.Include(x => x.Solicitudes)
				.Include(x => x.Yonkes)
				.FirstOrDefaultAsync(x => x.GuidId == guidId);
		}

		public async Task<SolicitudYonkes?> ObtenerPorIdAsync(int id)
		{
			return await _entities
					   .AsNoTracking()
					   .Include(x => x.Solicitudes)
					   .Include(x => x.Yonkes)
					   .Include(x => x.SolicitudYonkesEstatus)
					   .Include(x => x.SolicitudCotizaciones)
					   .FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<IList<SolicitudYonkes>> ObtenerPorSolicitudAsync(Guid SolicitudGuidId)
		{
			return await _entities
					  .AsNoTracking()
					  .Include(x => x.Yonkes)
					  .Include(x => x.SolicitudYonkesEstatus)
					  .Include(x => x.SolicitudCotizaciones)
					  .Where(x => x.SolicitudGuidId == SolicitudGuidId)
					  .OrderBy(x => x.FechaEnvio)
					  .ToListAsync();
		}


		//Verificar si existe 
		public async Task<IList<SolicitudYonkes>> ObtenerPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _entities
					   .AsNoTracking()
					   .Include(x => x.Solicitudes)
					   .Include(x => x.SolicitudYonkesEstatus)
					   .Where(x => x.YonkeGuidId == YonkeGuidId)
					   .OrderByDescending(x => x.FechaEnvio)
					   .ToListAsync();
		}


		public async Task<SolicitudYonkes?> ObtenerPorSolicitudGuidYonkeGuidAsync(Guid solicitudGuidId, Guid yonkeGuidId)
		{
			return await _entities
                	   .AsNoTracking()		
					   .Include(x => x.Solicitudes)
					   .Include(x => x.Yonkes)
					   .Include(x => x.SolicitudYonkesEstatus)
					   .Include(x => x.SolicitudCotizaciones)
					   .FirstOrDefaultAsync(x =>
						   x.Solicitudes.GuidId == solicitudGuidId &&
						   x.Yonkes.GuidId == yonkeGuidId);
		}

		public async Task<IList<SolicitudYonkes>> ObtenerPendientesPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _entities
						.AsNoTracking()
						.Include(x => x.Solicitudes)
						.Include(x => x.SolicitudYonkesEstatus)
						.Where(x =>
							x.YonkeGuidId == YonkeGuidId &&
							x.EstatusId == (int)SolicitudYonkeEstatusEnum.Enviada)
						.OrderBy(x => x.FechaEnvio)
						.ToListAsync();
		}

		public async Task<int> ContarPendientesPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _entities.CountAsync(x =>
						x.YonkeGuidId == YonkeGuidId &&
						x.EstatusId == (int)SolicitudYonkeEstatusEnum.Enviada);
		}

		/// <summary>
		/// reutilizable para todos los estados
		/// </summary>
		/// <param name="YonkeGuidId"></param>
		/// <param name="estatusId"></param>
		/// <returns></returns>
		public async Task<IList<SolicitudYonkes>> ObtenerPorYonkeYEstatusAsync(Guid YonkeGuidId, int estatusId)
		{
			return await _entities
						.AsNoTracking()
						.Include(x => x.Solicitudes)
						.Include(x => x.SolicitudYonkesEstatus)
						.Where(x =>
							x.YonkeGuidId == YonkeGuidId &&
							x.EstatusId == estatusId)
						.OrderByDescending(x => x.FechaEnvio)
						.ToListAsync();
		}

		/// <summary>
		/// Contar los enviados para vista del cliente
		/// </summary>
		/// <param name="SolicitudGuidId "></param>
		/// <returns></returns>
		public async Task<int> ContarEnviosAsync(Guid SolicitudGuidId)
		{
			return await _entities.CountAsync(x => x.SolicitudGuidId == SolicitudGuidId);
		}

		public async Task<int> ContarPorSolicitudYEstatusAsync(Guid SolicitudGuidId, int estatusId)
		{
			return await _entities.CountAsync(x => x.SolicitudGuidId == SolicitudGuidId && x.EstatusId == estatusId);
		}



		//La mas reciente solicutud de cada yonke nueva
		public async Task<SolicitudYonke_List_DTO?> SolicitudMasRecienteByYonke(Guid yonkeGuidId)
		{
			if (yonkeGuidId == Guid.Empty)
				return null;

			return await _context.SolicitudYonkes
		   .AsNoTracking()
		   .Where(x => x.YonkeGuidId == yonkeGuidId)
		   .OrderByDescending(x => x.FechaEnvio)
		   .Select(x => new SolicitudYonke_List_DTO
		   {
			   SolicitudYonkeGuidId = x.GuidId,
			   SolicitudGuidId = x.Solicitudes.GuidId,
			   Folio = x.Solicitudes.Folio,
			   PiezaBuscada = x.Solicitudes.PiezaBuscada,
			   NumeroParte = x.Solicitudes.NumeroParte,
			   FechaSolicitud = x.Solicitudes.FechaCreacion,
			   FechaEnvio = x.FechaEnvio,
			   EstatusId = x.EstatusId,
			   Estatus = x.SolicitudYonkesEstatus.EstatusSolicitud,
			   Vista = x.FechaVista.HasValue,
			   FechaVista = x.FechaVista,

			   Imagenes = x.Solicitudes.solicitudesImagenes
					   .Select(i => new SolicitudImagen_DTO
					   {
						   GuidId = i.GuidId,
						   Url = i.UrlImagen
					   })
					   .ToList(),

			   Ciudades = x.Solicitudes.SolicitudesCiudades
					   .Select(c => new SolicitudCiudad_DTO
					   {
						   CiudadId = c.CiudadId,
						   Ciudad = c.Ciudades.Ciudad
					   })
					   .ToList()
		   })
		   .FirstOrDefaultAsync();
		}

		//Todas las solicitudes de cada yonke logeado
		public async Task<List<SolicitudYonke_List_DTO>> ObtenerSolicitudesPorYonkeAsync(Guid yonkeGuidId, CancellationToken cancellationToken)
		{
			return await _context.SolicitudYonkes
		   .AsNoTracking()
		   .Where(x => x.YonkeGuidId == yonkeGuidId)
		   .OrderByDescending(x => x.FechaEnvio)
		   .Select(x => new SolicitudYonke_List_DTO
			   {
				   SolicitudYonkeGuidId = x.GuidId,
				   SolicitudGuidId = x.Solicitudes.GuidId,
				   Folio = x.Solicitudes.Folio,
				   PiezaBuscada = x.Solicitudes.PiezaBuscada,
				   NumeroParte = x.Solicitudes.NumeroParte,
				   FechaSolicitud = x.Solicitudes.FechaCreacion,
				   FechaEnvio = x.FechaEnvio,
				   EstatusId = x.EstatusId,
				   Estatus = x.SolicitudYonkesEstatus.EstatusSolicitud,
				   Vista = x.FechaVista.HasValue,
				   FechaVista = x.FechaVista,

				   Imagenes = x.Solicitudes.solicitudesImagenes
					   .Select(i => new SolicitudImagen_DTO
					   {
						   GuidId = i.GuidId,
						   Url = i.UrlImagen
					   })
					   .ToList(),

				   Ciudades = x.Solicitudes.SolicitudesCiudades
					   .Select(c => new SolicitudCiudad_DTO
					   {
						   CiudadId = c.CiudadId,
						   Ciudad = c.Ciudades.Ciudad
					   })
					   .ToList()
			   })
		   .ToListAsync(cancellationToken);
		}

		


		




	}
}
