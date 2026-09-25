using Core.Entitys;
using Core.Interfaces.Login;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Login
{
	public class ClienteOtpRepository : IClienteOtpRepository
	{
		private readonly AplicationDBContext _context;

		public ClienteOtpRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task<ClienteOtps?> ObtenerOtpActivoAsync(string telefono, CancellationToken cancellationToken = default)
		{
			return await _context.ClienteOtps
			.Where(x =>
				x.PhoneNumber == telefono &&
				x.Activo == true && 
				x.Usado == false)
			.OrderByDescending(x => x.FechaCreacion)
			.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task AgregarAsync(ClienteOtps otp, CancellationToken cancellationToken = default)
		{
			await _context.ClienteOtps.AddAsync(otp, cancellationToken);
		}

		public async Task DesactivarOtpsAnterioresAsync(string telefono, CancellationToken cancellationToken = default)
		{
			var otps = await _context.ClienteOtps
				.Where(x =>
					x.PhoneNumber == telefono &&
					x.Activo &&
					!x.Usado)
				.ToListAsync(cancellationToken);

			foreach (var otp in otps)
			{
				otp.Activo = false;
			}
		}

		public Task ActualizarAsync(ClienteOtps otp, CancellationToken cancellationToken = default)
		{
			_context.ClienteOtps.Update(otp);

			return Task.CompletedTask;
		}


		//Obtener el telfono si esta bloqueado
		public async Task<ClienteOtps?> ObtenerBloqueoTelefonoAsync(string telefono, CancellationToken cancellationToken = default)
		{
			return await _context.ClienteOtps
				.Where(x =>
					x.PhoneNumber == telefono &&
					x.BloqueadoHasta.HasValue &&
					x.BloqueadoHasta.Value > DateTime.UtcNow)
				.OrderByDescending(x => x.BloqueadoHasta)
				.FirstOrDefaultAsync(cancellationToken);
		}
	}
}
