using Core.Entitys;

namespace Core.Interfaces.Negocio.YunkeDispotivos
{
	public interface IYunkeDispositivoService
	{
		Task<YonkesDispositivos> ObtenerPorTokenAsync(string firebaseToken);
		Task AgregarAsync(YonkesDispositivos dispositivo, CancellationToken cancellationToken);
		Task ActualizarAsync(YonkesDispositivos dispositivo);

		Task<List<string>> ObtenerTokensFirebaseAsync(List<Guid> YonkeGuidIds);
	}
}
