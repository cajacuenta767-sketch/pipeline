using Core.DTO.Solicitudes.Estatus;
using Core.EntityBase;

namespace Core.Interfaces.Requests
{
	public interface ISolicitudesEstatusRepository<T> where T : BaseEntity
	{
		Task<IList<Solicitud_Estatus_List_DTO>> GetSolicitudEstatusAsync();
	}
}
