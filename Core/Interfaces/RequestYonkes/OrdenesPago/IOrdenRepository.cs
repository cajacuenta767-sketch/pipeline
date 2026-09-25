using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.OrdenesPago
{
	public interface IOrdenRepository
	{
		Task<Ordens?> ObtenerPorGuidAsync(
		   Guid guidId,
		   CancellationToken cancellationToken = default);

		Task<Ordens?> ObtenerPorCotizacionGuidAsync(
			Guid cotizacionGuidId,
			CancellationToken cancellationToken = default);

		Task<Ordens?> ObtenerPorUsuarioGuidAsync(
			Guid ordenGuidId,
			string usuarioId,
			CancellationToken cancellationToken = default);

		Task AgregarAsync(
			Ordens orden,
			CancellationToken cancellationToken = default);

		Task ActualizarAsync(
			Ordens orden,
			CancellationToken cancellationToken = default);

		Task<bool> ExistePorCotizacionAsync(
			Guid cotizacionGuidId,
			CancellationToken cancellationToken = default);
	}
}
