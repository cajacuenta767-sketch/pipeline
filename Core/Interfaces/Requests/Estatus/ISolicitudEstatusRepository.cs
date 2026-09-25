using Core.DTO.Solicitudes.Estatus;

namespace Core.Interfaces.Requests.Estatus
{
	public interface ISolicitudEstatusRepository
	{
		Task<IList<Solicitud_Estatus_List_DTO>> GetSolicitudEstatusAsync();
	}
}
