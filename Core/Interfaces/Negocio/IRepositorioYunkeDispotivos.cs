using Core.Entitys;

namespace Core.Interfaces.Negocio
{
	public interface IRepositorioYunkeDispotivos
	{
		Task<YonkesDispositivos?> ObtenerPorTokenAsync(string firebaseToken);
		Task AgregarAsync(YonkesDispositivos entity);
		Task ActualizarAsync(YonkesDispositivos entity);
	}
}
