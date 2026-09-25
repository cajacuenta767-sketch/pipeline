using Core.Entitys;
using Core.Interfaces.RequestYonkes;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.RequestYonkes
{
	public class OrdenPagoRepository : IOrdenRepository
	{
		private readonly AplicationDBContext _context;

		private DbSet<Ordens> _entities => _context.Set<Ordens>();


		public OrdenPagoRepository(AplicationDBContext context)
		{
			_context = context;
		}


		// ==========================================
		// OBTENER POR GUID
		// ==========================================

		public async Task<Ordens?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.GuidId == guidId,
					cancellationToken);
		}


		// ==========================================
		// OBTENER POR COTIZACIÓN
		// ==========================================

		public async Task<Ordens?> ObtenerPorCotizacionGuidAsync(Guid cotizacionGuidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.CotizacionGuidId == cotizacionGuidId,
					cancellationToken);
		}


		// ==========================================
		// OBTENER ORDEN DEL USUARIO
		// ==========================================

		public async Task<Ordens?> ObtenerPorUsuarioGuidAsync(Guid ordenGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x =>
						x.GuidId == ordenGuidId &&
						x.UsuarioId == usuarioId,
					cancellationToken);
		}


		// ==========================================
		// AGREGAR
		// ==========================================

		public async Task AgregarAsync(Ordens orden, CancellationToken cancellationToken = default)
		{
			await _entities.AddAsync(
				orden,
				cancellationToken);
		}


		// ==========================================
		// ACTUALIZAR
		// ==========================================

		public Task ActualizarAsync(Ordens orden, CancellationToken cancellationToken = default)
		{
			_entities.Update(orden);

			return Task.CompletedTask;
		}


		// ==========================================
		// EXISTE COTIZACIÓN
		// ==========================================

		public async Task<bool> ExistePorCotizacionAsync(Guid cotizacionGuidId, CancellationToken cancellationToken = default)
		{
			return await _entities
				.AsNoTracking()
				.AnyAsync(
					x => x.CotizacionGuidId == cotizacionGuidId,
					cancellationToken);
		}
	}



}
