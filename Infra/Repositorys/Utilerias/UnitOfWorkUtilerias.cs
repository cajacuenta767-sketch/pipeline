using AutoMapper;
using Core.Entitys;
using Core.Interfaces.Utilerias;
using Infra.DataContext;

namespace Infra.Repositorys.Utilerias
{
	public class UnitOfWorkUtilerias : IUnitOfWorkUtilerias
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		//Mapper 
		private readonly IMapper _mapper;


		private readonly IRepositorioUtilerias<Entidades> _entidadesRepository;
		private readonly IRepositorioUtilerias<Ciudades> _ciudadesRepository;
		private readonly IRepositorioUtilerias<Marcas> _marcasRepository;
		private readonly IRepositorioUtilerias<Modelos> _modelosRepository;


		public UnitOfWorkUtilerias(AplicationDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}



		public IRepositorioUtilerias<Entidades> EntidadesRepository => _entidadesRepository ?? new UtileriaRepository<Entidades>(_context, _mapper);
		public IRepositorioUtilerias<Ciudades> CiudadesRepository => _ciudadesRepository ?? new UtileriaRepository<Ciudades>(_context, _mapper);
		public IRepositorioUtilerias<Marcas> MarcasRepository => _marcasRepository ?? new UtileriaRepository<Marcas>(_context, _mapper);
		public IRepositorioUtilerias<Modelos> ModelosRepository => _modelosRepository ?? new UtileriaRepository<Modelos>(_context, _mapper);






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

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		#endregion
	}
}
