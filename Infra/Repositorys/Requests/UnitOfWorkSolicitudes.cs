using AutoMapper;
using Core.Entitys;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.historials;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Repositorys.Requests
{
	public class UnitOfWorkSolicitudes : IUnitOfWorkSolicitudes
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		//Mapper 
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;


		private readonly ISolicitudesRepository<Solicitudes> _solicitudesRepository;
		private readonly ISolicitudesEstatusRepository<SolicitudesEstatus> _solicitudesEstatusRepository;
		private readonly ISolicitudImagenesRepository<SolicitudesImagenes> _solicitudesImagenesRepository;
		private readonly ISolicitudHistorialRepository<SolicitudesHistorials> _solicitudesHistorialRepository;
		private ISolicitudCiudadesRepository? _solicitudesCiudadesRepository;

		public UnitOfWorkSolicitudes(AplicationDBContext context,
									IMapper mapper,
									ICurrentUserService currentUserService)
		{
			_context = context;
			_mapper = mapper;
			_currentUserService = currentUserService;
		}

		public ISolicitudesRepository<Solicitudes> SolicitudesRepository => _solicitudesRepository ?? new SolicitudesRepository<Solicitudes>(_context, _mapper, _currentUserService);
		public ISolicitudesEstatusRepository<SolicitudesEstatus> solicitudesEstatusRepository => _solicitudesEstatusRepository ?? new SolicitudesEstatusRepository<SolicitudesEstatus>(_context, _mapper);
		public ISolicitudImagenesRepository<SolicitudesImagenes> solicitudImagenesRepository => _solicitudesImagenesRepository ?? new SolicitudesImagenesRepository<SolicitudesImagenes>(_context, _mapper);
		public ISolicitudHistorialRepository<SolicitudesHistorials> solicitudHistorialRepository => _solicitudesHistorialRepository ?? new SolicitudHistorialRepository<SolicitudesHistorials>(_context, _mapper);
		public ISolicitudCiudadesRepository SolicitudCiudadesRepository => _solicitudesCiudadesRepository ??= new SolicitudCiudadesRepository(_context);

		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await _context.Database.BeginTransactionAsync();
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

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken)
		{
			await _context.SaveChangesAsync(cancellationToken);
		}
		#endregion
	}
}
