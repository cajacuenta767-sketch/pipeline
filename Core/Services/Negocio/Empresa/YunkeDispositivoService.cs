using Core.Entitys;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.YunkeDispotivos;

namespace Core.Services.Negocio.Empresa
{
	public class YunkeDispositivoService : IYunkeDispositivoService
	{
		private readonly IUnitOfWorkNegocio _unitOfWorkNegocio;


		public YunkeDispositivoService(IUnitOfWorkNegocio unitOfWorkNegocio)
		{
			_unitOfWorkNegocio = unitOfWorkNegocio;
		}


		public async Task<YonkesDispositivos> ObtenerPorTokenAsync(string firebaseToken)
		{
			return await _unitOfWorkNegocio.YunkeDispositivoRepository.ObtenerPorTokenAsync(firebaseToken);	
		}

		public async Task AgregarAsync(YonkesDispositivos dispositivo, CancellationToken cancellationToken)
		{
			dispositivo.FechaRegistro = DateTime.UtcNow;
			dispositivo.Activo = true;

			await _unitOfWorkNegocio.YunkeDispositivoRepository
				.AgregarAsync(dispositivo, cancellationToken);

			await _unitOfWorkNegocio.SaveChangesAsync();
		}

		public async Task ActualizarAsync(YonkesDispositivos dispositivo)
		{
			await _unitOfWorkNegocio.YunkeDispositivoRepository
				.ActualizarAsync(dispositivo);

			await _unitOfWorkNegocio.SaveChangesAsync();
		}

		public async Task<List<string>> ObtenerTokensFirebaseAsync(List<Guid> YonkeGuidIds)
		{
			return await _unitOfWorkNegocio.YunkeRepository.ObtenerTokensFirebaseAsync(YonkeGuidIds);
		}

	}
}
