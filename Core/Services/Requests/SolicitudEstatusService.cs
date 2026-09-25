using AutoMapper;
using Core.DTO.Solicitudes.Estatus;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.Estatus;

namespace Core.Services.Requests
{
	public class SolicitudEstatusService : ISolicitudEstatusService
	{
		public readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;
		public readonly IMapper _mapper;


		public SolicitudEstatusService(IUnitOfWorkSolicitudes unitOfWorkSolicitudes, IMapper mapper)
		{
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;
			_mapper = mapper;
		}


		public async Task<IList<Solicitud_Estatus_List_DTO>> GetSolicitudEstatusAsync()
		{
			return await _unitOfWorkSolicitudes.solicitudesEstatusRepository.GetSolicitudEstatusAsync();
		}
	}
}
