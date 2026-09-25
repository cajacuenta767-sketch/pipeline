using Core.DTO.Utilerias.Marcas;
using Core.Entitys;

namespace Core.Interfaces.Utilerias.brand
{
	public interface IMarcaReposotorio
	{
		Task<IList<MarcasListDTO>> getMarcasListByEmpresa();
		Task<Marcas> getMarcaById(int id);
		Task NuevoMarca(Marcas marcaSave);
		Task<bool> UpdateMarca(Marcas marcaUpdate);


		Task<bool> ExisteMarca(int id);
	}
}
