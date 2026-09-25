using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.CotizacionMessages
{
	public interface ISolicitudCotizacionMensajeRepository
	{
		Task<SolicitudCotizacionMensajes?> ObtenerPorGuidIdAsync(Guid guidId, CancellationToken cancellationToken = default);

		Task<List<SolicitudCotizacionMensajes>> ObtenerPorCotizacionAsync(Guid solicitudCotizacionGuidId, CancellationToken cancellationToken = default);

		Task<List<SolicitudCotizacionMensajes>> ObtenerNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);

		Task<int> ObtenerCantidadNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);

		Task AgregarAsync(SolicitudCotizacionMensajes mensaje, CancellationToken cancellationToken = default);

		Task MarcarComoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);
	}
}
