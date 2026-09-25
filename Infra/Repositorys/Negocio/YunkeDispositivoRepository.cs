using Core.Entitys;
using Core.Interfaces.Negocio.YunkeDispotivos;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Negocio
{
	public class YunkeDispositivoRepository : IYunkeDispositivoRepository
	{
		private readonly AplicationDBContext _context;
		public YunkeDispositivoRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task<YonkesDispositivos?> ObtenerPorTokenAsync(string firebaseToken)
		{
			return await _context.YonkesDispositivos
						.FirstOrDefaultAsync(x => x.FirebaseToken == firebaseToken);
		}

		public async Task AgregarAsync(YonkesDispositivos entitty, CancellationToken cancellation)
		{
			await _context.YonkesDispositivos.AddAsync(entitty, cancellation);
		}

		public Task ActualizarAsync(YonkesDispositivos dispositivo)
		{
			_context.YonkesDispositivos.Update(dispositivo);
			return Task.CompletedTask;
		}

		
	}
}
