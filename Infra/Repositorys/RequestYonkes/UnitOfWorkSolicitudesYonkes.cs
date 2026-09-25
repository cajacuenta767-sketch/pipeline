using AutoMapper;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.CotizacionMessages;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Repositorys.RequestYonkes
{
	public class UnitOfWorkSolicitudesYonkes : IUnitOfWorkSolicitudYonkes
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		//Mapper 
		private readonly IMapper _mapper;


		private readonly ISolicitudesYonkeRepository _solicitudYonkeRepository;
		private readonly ISolicitudCotizacionRepository _solicitudCotizacionRepository;
		private readonly ISolicitudCotizacionImagenesRepository _solicitudCotizacionImagenesRepository;

		private readonly ISolicitudCotizacionMensajeRepository _solicitudCotizacionMensajeRepository;

		private readonly IOrdenRepository _ordenRepository;
		private readonly IPagosRepository _pagosRepository;	 



		public UnitOfWorkSolicitudesYonkes(AplicationDBContext context,
									IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public ISolicitudesYonkeRepository SolicitudYonkeRepository => _solicitudYonkeRepository ?? new SolicitudYonkeRepository(_context);
		public ISolicitudCotizacionRepository SolicitudCotizacionRepository => _solicitudCotizacionRepository ?? new CotizacionYonkeRepository(_context);

		public ISolicitudCotizacionImagenesRepository solicitudCotizacionImagenesRepository => _solicitudCotizacionImagenesRepository ?? new CotizacionImagenesRepository(_context);



		public ISolicitudCotizacionMensajeRepository SolicitudCotizacionMensajeRepository => _solicitudCotizacionMensajeRepository ?? new SolicitudCotizacionMensajeRepository(_context);

		public IOrdenRepository OrdenRepository => _ordenRepository ?? new OrdenPagoRepository (_context);
		public IPagosRepository pagosRepository => _pagosRepository ?? new PagoRepository(_context);




		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
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
