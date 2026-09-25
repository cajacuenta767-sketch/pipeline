using Core.DTO.Utilerias.Ciudades;
using Core.Entitys;

namespace Core.Interfaces.Utilerias.Citys
{
    public interface ICiudadRepositorio
    {
		Task<IList<CiudadesListDTO>> getCiudades(int entidadId);
		Task<IList<int>> getCiudadesListadoId();

		Task<Ciudades> getCiudadById(int id);
	}
}
