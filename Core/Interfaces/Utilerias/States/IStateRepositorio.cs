using Core.DTO.Utilerias.States;
using Core.Entitys;

namespace Core.Interfaces.Utilerias.States
{
	public interface IStateRepositorio
	{
		Task<IList<EntidadesListDTO>> getStates();

		Task<Entidades> getStateById(int id);
	}
}
