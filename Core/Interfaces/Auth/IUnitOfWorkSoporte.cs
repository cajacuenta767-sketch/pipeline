using Core.Entitys;

namespace Core.Interfaces.Auth
{
	public interface IUnitOfWorkSoporte : IDisposable
	{
		ISoporteRepository<AccesoDetalles> AccesoDetalleRepository { get; }




		void SaveChanges();

		Task SaveChangesAsync();
	}
}
