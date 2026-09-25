using Core.DTO.Utilerias.Ciudades;
using Core.DTO.Utilerias.Marcas;
using Core.DTO.Utilerias.Models;
using Core.DTO.Utilerias.States;
using Core.EntityBase;
using Core.Entitys;

namespace Core.Interfaces.Utilerias
{
    public interface IRepositorioUtilerias<T> where T : BaseEntity
    {
		Task<IList<EntidadesListDTO>> getEntidadesList();
		Task<Entidades> getEntidadById(int id);


		Task<IList<CiudadesListDTO>> getCiudadesList(int entidadId);
		Task<IList<int>> getListadoCiudadesId();
		Task<Ciudades> getCiudadById(int id);



		Task<IList<MarcasListDTO>> getMarcas();
		Task<Marcas> getMarcaById (int id);
		Task addMarca(Marcas marcaSave);
		void updataMarca(Marcas marcaUpdate);
		Task<bool> ExisteMarca(int id);



		Task<IList<ModelosListDTO>> getModelos(int marcaId);
		Task<Modelos> getModeloById(int id);
		Task addModelo(Modelos modeloSave);
		void updataModelo(Modelos modeloUpdate);
		Task<bool> ExisteModelo(int id);
	}
}
