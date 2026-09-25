using Core.Entitys;

namespace Core.Interfaces.Auth.AccesoDetalle
{
	public interface IAccesoDetalleService
	{
		Task<IList<AccesoDetalles>> GetAccesoDetalles();
		Task<AccesoDetalles> GetAccesoDetalle(int id);
		Task InsertAccesoDetalle(AccesoDetalles accesoDetalles);
		Task<bool> UpdateAccesoDetalle(AccesoDetalles accesoDetalles);
	}
}
