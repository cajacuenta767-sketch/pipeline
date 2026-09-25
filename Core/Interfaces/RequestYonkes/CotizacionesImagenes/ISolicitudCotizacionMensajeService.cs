using Core.DTO.SolicitudMenssages;

namespace Core.Interfaces.RequestYonkes.CotizacionesImagenes
{
	public interface ISolicitudCotizacionMensajeService
	{
		Task<SolicitudCotizacionMensajeDTO> EnviarMensajeAsync(RegistrarMensajeCotizacionRequest request, Guid usuarioId, CancellationToken cancellationToken = default);

		Task<List<SolicitudCotizacionMensajeDTO>> ObtenerMensajesAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);

		Task MarcarComoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);

		Task<int> ObtenerCantidadNoLeidosAsync(Guid solicitudCotizacionGuidId, Guid usuarioId, CancellationToken cancellationToken = default);


	}
}
