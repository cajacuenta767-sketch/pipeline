using Core.Entitys;
using Core.Interfaces.RequestYonkes.CotizacionMessages;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.RequestYonkes
{
	public class SolicitudCotizacionMensajeRepository : ISolicitudCotizacionMensajeRepository
	{
		private readonly AplicationDBContext _context;

		public SolicitudCotizacionMensajeRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task<SolicitudCotizacionMensajes?> ObtenerPorGuidIdAsync(
			Guid guidId,
			CancellationToken cancellationToken = default)
		{
			return await _context.SolicitudCotizacionMensajes
				.FirstOrDefaultAsync(
					x => x.GuidId == guidId,
					cancellationToken);
		}

		public async Task<List<SolicitudCotizacionMensajes>> ObtenerPorCotizacionAsync(Guid solicitudCotizacionGuidId, CancellationToken cancellationToken = default)
		{
			return await _context.SolicitudCotizacionMensajes
				.AsNoTracking()
				.Where(x =>
					x.SolicitudCotizacionGuidId == solicitudCotizacionGuidId)
				.OrderBy(x => x.FechaCreacion)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<SolicitudCotizacionMensajes>> ObtenerNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			return await _context.SolicitudCotizacionMensajes
				.AsNoTracking()
				.Where(x =>
					x.SolicitudCotizacionGuidId == solicitudCotizacionGuidId &&
					x.UsuarioId != usuarioId &&
					!x.Leido)
				.OrderBy(x => x.FechaCreacion)
				.ToListAsync(cancellationToken);
		}

		public async Task<int> ObtenerCantidadNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			return await _context.SolicitudCotizacionMensajes
				.CountAsync(
					x =>
						x.SolicitudCotizacionGuidId == solicitudCotizacionGuidId &&
						x.UsuarioId != usuarioId &&
						!x.Leido,
					cancellationToken);
		}

		public async Task AgregarAsync(SolicitudCotizacionMensajes mensaje, CancellationToken cancellationToken = default)
		{
			await _context.SolicitudCotizacionMensajes.AddAsync(mensaje, cancellationToken);
		}

		public async Task MarcarComoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			await _context.SolicitudCotizacionMensajes
				.Where(x =>
					x.SolicitudCotizacionGuidId == solicitudCotizacionGuidId &&
					x.UsuarioId != usuarioId &&
					!x.Leido)
				.ExecuteUpdateAsync(
					setters => setters
						.SetProperty(x => x.Leido, true)
						.SetProperty(
							x => x.FechaLectura,
							DateTime.UtcNow),
					cancellationToken);
		}
	}
}
