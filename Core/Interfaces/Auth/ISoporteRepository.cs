using Core.EntityBase;
using Core.Entitys;

namespace Core.Interfaces.Auth
{
	public interface ISoporteRepository<T> where T : BaseEntity
	{


		//******************************Acceso Detalles****
		Task<IEnumerable<AccesoDetalles>> GetAllAccesoDetallesAsync();
		Task<AccesoDetalles> GetAccesoDetallesById(int id);
		Task AddAccesoDetalle(AccesoDetalles accesoDetalles);
		void UpdateAccesoDetalle(AccesoDetalles accesoDetalles);


	}
}
