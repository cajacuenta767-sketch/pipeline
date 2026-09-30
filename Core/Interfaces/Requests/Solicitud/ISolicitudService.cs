using Core.DTO.Solicitudes.Requests;
using Core.DTO.SolocitudCotizaciones;
using Core.Entitys;

namespace Core.Interfaces.Requests.Solicitud
{
	public interface ISolicitudService
	{		
		IQueryable<Solicitud_Busqueda_DTO> GetSolicitudesByUserId(DateTime desde, DateTime hasta, Guid userId);
		Task<Solicitud_Busqueda_DTO> GetSolicitudByGuidId(Guid guidId);
		Task<Solicitudes?> GetByGuidId(Guid guidId);
		Task<Solicitudes> NuevaSolicitud(Solicitudes_Create_DTO solicitudSave, CancellationToken cancellationToken);
		Task<bool> UpdateSolcitud(Solicitudes solicitud);
		Task<string> UpdateStatusAsync(SolicitudUpdateStatusDTO dto);

		// Contar solicitudes del usuario con estatus 1, 2, 3 y 4
		Task<int> ContarSolicitudesUsuarioAsync();
		IQueryable<Solicitud_Busqueda_DTO> VerSolicitudesByUserDashboard();



		//Contar las cotizaciones que tiene el cliente
		Task<int> ContarCotizacionesUsuarioAsync();		
		IQueryable<Cotizacion_List_Dashboad_DTO> GetCotizacionesByUserId();

		// Solicitud más reciente del cliente
		Task<Solicitud_Busqueda_DTO?> ObtenerSolicitudMasRecienteAsync();

	}
}
