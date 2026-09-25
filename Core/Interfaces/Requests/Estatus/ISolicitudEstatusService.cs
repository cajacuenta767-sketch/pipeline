using Core.DTO.Solicitudes.Estatus;

namespace Core.Interfaces.Requests.Estatus
{
	public interface ISolicitudEstatusService
	{
		Task<IList<Solicitud_Estatus_List_DTO>> GetSolicitudEstatusAsync();
	}
}
