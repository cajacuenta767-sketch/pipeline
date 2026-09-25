using Core.DTO.Utilerias.Models;
using Core.Entitys;

namespace Core.Interfaces.Utilerias.Models
{
	public interface IModeloService
	{
		Task<IList<ModelosListDTO>> getModelosListByEmpresa(int marcaId);
		Task<Modelos> getModeloById(int id);
		Task NuevoModelo(Modelos modeloSave);
		Task<bool> UpdateModelo(Modelos modeloUpdate);


		Task<bool> ExisteModelo(int id);
	}
}
