using AutoMapper;
using Core.Entitys;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.Calificacion;
using Core.Interfaces.Negocio.Coberturas;
using Core.Interfaces.Negocio.YunkeDispotivos;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Repositorys.Negocio
{
	public class UnitOfWorkNegocio : IUnitOfWorkNegocio
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		//Mapper 
		private readonly IMapper _mapper;


		private readonly IRepositrioYunke<Yonkes>? _yunkeRepository;
		private IYunkeCoberturaRepository? _yunkeCoberturaRepository;
		private IYunkeDispositivoRepository? _yunkeDispositivoRepository;
		private IYunkeCalificacionRepository? _yunkeCalificacionRepository;

		public UnitOfWorkNegocio(AplicationDBContext context,
							     IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		public IRepositrioYunke<Yonkes> YunkeRepository => _yunkeRepository ?? new YunkeRepository<Yonkes>(_context, _mapper);
	
		public IYunkeCoberturaRepository YunkeCoberturaRepository => _yunkeCoberturaRepository ??= 	new YunkeCoberturaRepository(_context);

		public IYunkeDispositivoRepository YunkeDispositivoRepository => _yunkeDispositivoRepository ??= new YunkeDispositivoRepository(_context);

		public IYunkeCalificacionRepository YunkeCalificacionRepository => _yunkeCalificacionRepository ?? new YunkeCalificacionRepository(_context);


		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Database.BeginTransactionAsync(cancellationToken);
		}

		







		#region Generados en defualt
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
