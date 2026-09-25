using Core.DTO.Utilerias.States;
using Core.Entitys;
using Core.Interfaces.Utilerias;
using Core.Interfaces.Utilerias.States;

namespace Core.Services.Utilerias.States
{
	public class EntidadService : IStateService
	{
		public readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public EntidadService(IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWorkUtilerias = unitOfWorkUtilerias;
		}


		public async Task<Entidades> getStateById(int id)
		{
			return await _unitOfWorkUtilerias.EntidadesRepository.getEntidadById(id);
		}

		public async Task<IList<EntidadesListDTO>> getStates()
		{
			return await _unitOfWorkUtilerias.EntidadesRepository.getEntidadesList();
		}
	}
}
