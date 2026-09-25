using Core.Entitys;

namespace Core.Interfaces.Negocio
{
	public interface IRepositorioYonkeCalificaciones
	{
		Task<bool> ExisteCalificacionAsync(Guid cotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);

		Task AgregarAsync(YonkesCalificaciones calificacion, CancellationToken cancellationToken = default);

		Task<decimal> ObtenerPromedioAsync(Guid yonkeGuidId, CancellationToken cancellationToken = default);

		Task<int> ObtenerTotalAsync(Guid yonkeGuidId, CancellationToken cancellationToken = default);
	}
}
