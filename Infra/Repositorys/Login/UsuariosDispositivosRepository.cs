using Core.Entitys;
using Core.Interfaces.Login;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Login
{
	public class UsuariosDispositivosRepository : IUsuariosDispositivosRepository
	{
		private readonly AplicationDBContext _context;

		public UsuariosDispositivosRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task<UsuariosDispositivos?> ObtenerPorTokenAsync(string firebaseToken, CancellationToken cancellationToken = default)
		{
			return await _context.UsuariosDispositivos
				.FirstOrDefaultAsync(x =>
					x.FirebaseToken == firebaseToken,
					cancellationToken);
		}

		public async Task<List<UsuariosDispositivos>> ObtenerTokensUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default)
		{
			return await _context.UsuariosDispositivos
				.Where(x =>
					x.UsuarioId == usuarioId &&
					x.Activo)
				.ToListAsync(cancellationToken);
		}

		public async Task AgregarAsync(UsuariosDispositivos dispositivo, CancellationToken cancellationToken = default)
		{
			await _context.UsuariosDispositivos
				.AddAsync(dispositivo, cancellationToken);
		}

		public void Actualizar(UsuariosDispositivos dispositivo)
		{
			_context.UsuariosDispositivos.Update(dispositivo);
		}
	}
}
