using Core.DTO.Solicitudes.Requests;
using Core.Entitys;

namespace Core.Interfaces.Requests.Solicitud
{
	public interface ISolicitudRepository
	{
		Task<IQueryable<Solicitud_Busqueda_DTO>> GetSolicitudesByUserId(DateTime desde, DateTime hasta, Guid userId);
		Task<Solicitud_Busqueda_DTO> GetSolicitudByGuidId(Guid guidId);
		Task<Solicitudes?> GetByGuidId(Guid guidId);
		Task<Solicitudes> NuevaSolicitud(Solicitudes_Create_DTO solicitudSave, CancellationToken cancellationToken);
		Task<bool> UpdateSolcitud(Solicitudes solicitud, CancellationToken cancellationToken);
		Task<string> UpdateStatusAsync(SolicitudUpdateStatusDTO dto, CancellationToken cancellationToken);
	}
}
