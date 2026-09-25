using Core.Entitys;

namespace Core.Interfaces.Negocio.YunkeDispotivos
{
	public interface IYunkeDispositivoRepository
	{	
		Task<YonkesDispositivos> ObtenerPorTokenAsync(string firebaseToken);
		Task AgregarAsync(YonkesDispositivos dispositivo, CancellationToken cancellation);
		Task ActualizarAsync(YonkesDispositivos dispositivo);
	}
}
