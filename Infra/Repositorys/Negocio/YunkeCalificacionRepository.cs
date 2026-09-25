using Core.Entitys;
using Core.Interfaces.Negocio.Calificacion;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Negocio
{
	public class YunkeCalificacionRepository : IYunkeCalificacionRepository
	{
		private readonly AplicationDBContext _context;

		public YunkeCalificacionRepository(	AplicationDBContext context)
		{
			_context = context;
		}


		public async Task<bool> ExisteCalificacionAsync(Guid cotizacionGuidId, Guid? usuarioId,  CancellationToken cancellationToken = default)
		{
			return await _context.YonkesCalificaciones
				.AsNoTracking()
				.AnyAsync(
					x =>
						x.CotizacionGuidId == cotizacionGuidId &&
						x.UsuarioId == usuarioId &&
						x.Activa,
					cancellationToken);
		}

		public async Task AgregarAsync(YonkesCalificaciones calificacion, CancellationToken cancellationToken = default)
		{
			await _context.YonkesCalificaciones
				.AddAsync(
					calificacion,
					cancellationToken);
		}

		public async Task<decimal> ObtenerPromedioAsync(Guid yonkeGuidId, CancellationToken cancellationToken = default)
		{
			var promedio = await _context.YonkesCalificaciones
				.AsNoTracking()
				.Where(x =>
					x.YonkeGuidId == yonkeGuidId &&
					x.Activa)
				.Select(x => (decimal?)x.Calificacion)
				.AverageAsync(cancellationToken);

			return promedio.HasValue
				? Math.Round(promedio.Value, 1)
				: 0;
		}

		public async Task<int> ObtenerTotalAsync(Guid yonkeGuidId, CancellationToken cancellationToken = default)
		{
			return await _context.YonkesCalificaciones
				.AsNoTracking()
				.CountAsync(
					x =>
						x.YonkeGuidId == yonkeGuidId &&
						x.Activa,
					cancellationToken);
		}
	}
}
