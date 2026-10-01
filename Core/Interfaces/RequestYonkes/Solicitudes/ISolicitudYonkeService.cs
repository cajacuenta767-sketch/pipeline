using Core.DTO.SolicitudYonkes;

namespace Core.Interfaces.RequestYonkes.Solicitudes
{
	public interface ISolicitudYonkeService
	{
		Task<int> EnviarSolicitudAsync(Guid solicitudGuidId, CancellationToken cancellationToken);
		Task MarcarComoVistaAsync(Guid solicitudYonkeGuidId, CancellationToken cancellationToken);

		//Task RegistrarCotizacionAsync(Guid solicitudYonkeGuidId, RegistrarCotizacionRequest request, CancellationToken cancellation);

		Task<int> ContarPendientesPorYonkeAsync(Guid YonkeGuidId);


		//Solicitud mas reciente de cada yonke logeado
		Task<SolicitudYonke_List_DTO?> SolicitudRecienteByYonke(Guid YonkeGuidId);

		//Solicitudes de cada yonke
		Task<List<SolicitudYonke_List_DTO>> ObtenerSolicitudesPorYonkeAsync(Guid YonkeGuidId, CancellationToken cancellationToken);

		Task<List<SolicitudYonkeDestinatarioDTO>> ObtenerDestinatariosPorSolicitudAsync(Guid solicitudGuidId, CancellationToken cancellationToken);
	}
}
