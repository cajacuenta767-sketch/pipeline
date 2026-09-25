using AutoMapper;
using Core.Interfaces.Login;
using Core.Interfaces.Login_Cliente;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Repositorys.Login
{
	public class UnitOfWorkClientes : IUnitOfWorkCliente
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		//Mapper 
		private readonly IMapper _mapper;

		private ILoginRepositorio? _loginRepositorio;
		private IClienteOtpRepository? _clienteOtpRepository;
		private IUsuariosDispositivosRepository? _usuariosDispositivosRepository;

		public UnitOfWorkClientes(AplicationDBContext context,
							   IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public ILoginRepositorio LoginRepositorio => _loginRepositorio ?? new LoginClienteRepository(_context);

		public IClienteOtpRepository ClienteOtpRepository => _clienteOtpRepository ?? new ClienteOtpRepository(_context);

		public IUsuariosDispositivosRepository UsuariosDispositivosRepository => _usuariosDispositivosRepository ?? new UsuariosDispositivosRepository(_context);




		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Database.BeginTransactionAsync(cancellationToken);
		}


		#region MyRegion
		public void Dispose()
		{
			if (_context != null)
			{
				_context.Dispose();
			}
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}
		#endregion

	}
}
