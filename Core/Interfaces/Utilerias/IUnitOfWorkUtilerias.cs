using Core.Entitys;

namespace Core.Interfaces.Utilerias
{
    public interface IUnitOfWorkUtilerias : IDisposable
	{
		IRepositorioUtilerias<Entidades> EntidadesRepository { get; }
		IRepositorioUtilerias<Ciudades> CiudadesRepository { get; }
		IRepositorioUtilerias<Marcas> MarcasRepository { get; }
		IRepositorioUtilerias<Modelos> ModelosRepository { get; }

		void SaveChanges();

		Task SaveChangesAsync();
	}
}
