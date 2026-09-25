using AutoMapper;
using Core.Entitys;
using Core.Interfaces.Auth;
using Infra.DataContext;
using Infra.Repositorys.Soporte.Empresa;

namespace Infra.Repositorys.Soporte
{
	public class UnitOfWorkSoporte : IUnitOfWorkSoporte
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		private readonly IMapper _mapper;


		#region Soporte
		private readonly ISoporteRepository<Yonkes> _Yonkes;
		#endregion

		#region Acceso Detalles
		private readonly ISoporteRepository<AccesoDetalles> _accesoDetalles;
		#endregion


		public UnitOfWorkSoporte(AplicationDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		#region Soporte
		public ISoporteRepository<Yonkes> YunkeRepository => _Yonkes ?? new SoporteRepository<Yonkes>(_context, _mapper);

		#endregion

		#region Acceso Detalles
		public ISoporteRepository<AccesoDetalles> AccesoDetalleRepository => _accesoDetalles ?? new SoporteRepository<AccesoDetalles>(_context, _mapper);
		#endregion




		#region Generados por Default
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

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		#endregion
	}
}
