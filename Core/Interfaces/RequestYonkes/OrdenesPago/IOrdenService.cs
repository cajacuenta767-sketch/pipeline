using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.OrdenesPago
{
	public interface IOrdenService
	{
		Task<Ordens?> ObtenerPorGuidAsync(
			Guid guidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task<Ordens?> ObtenerPorCotizacionAsync(
			Guid cotizacionGuidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task<Ordens> CrearDesdeCotizacionAsync(
			Guid cotizacionGuidId,
			Guid usuarioId,
			CancellationToken cancellationToken = default);

		Task<Ordens> ActualizarEstatusAsync(
			Guid ordenGuidId,
			Guid usuarioId,
			int estatusOrdenId,
			CancellationToken cancellationToken = default);
	}
}
