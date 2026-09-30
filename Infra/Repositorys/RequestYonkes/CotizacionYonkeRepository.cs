using Core.Entitys;
using Core.Interfaces.RequestYonkes;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.RequestYonkes
{
	public class CotizacionYonkeRepository : ISolicitudCotizacionRepository
	{
		private readonly AplicationDBContext _context;

		public CotizacionYonkeRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task AgregarAsync(SolicitudCotizaciones cotizacion)
		{
			await _context.SolicitudCotizaciones.AddAsync(cotizacion);
		}


		public Task ActualizarAsync(SolicitudCotizaciones cotizacion)
		{
			_context.SolicitudCotizaciones.Update(cotizacion);
			return Task.CompletedTask;
		}



		public async Task<bool> ExisteCotizacionAsync(Guid solicitudYonkeGuidId)
		{
			return await _context.SolicitudCotizaciones
				.AnyAsync(x =>
					x.SolicitudYonkeGuidId == solicitudYonkeGuidId &&
					x.Activo);
		}

		public async Task<SolicitudCotizaciones?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken)
		{			
			return await _context.SolicitudCotizaciones
			   .Include(x => x.SolicitudCotizacionesImagenes)
			   .Include(x => x.SolicitudYonkes)
				   .ThenInclude(x => x.Solicitudes)
			   .FirstOrDefaultAsync(x => x.GuidId == guidId,   cancellationToken);
		}

		public async Task<int> ContarPorYonkeAsync(Guid yonkeGuidId, CancellationToken cancellationToken)
		{
			return await _context.SolicitudCotizaciones
				.AsNoTracking()
				.CountAsync(
					x => x.SolicitudYonkes.YonkeGuidId == yonkeGuidId,
					cancellationToken);
		}
	}
}
