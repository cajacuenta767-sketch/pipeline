using Core.Interfaces.Login;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces.Login_Cliente
{
	public interface IUnitOfWorkCliente : IDisposable
	{
		ILoginRepositorio LoginRepositorio { get; }
		IClienteOtpRepository ClienteOtpRepository { get; }
		IUsuariosDispositivosRepository UsuariosDispositivosRepository { get; }

		Task<int> SaveChangesAsync();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);


		Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
	}
}
