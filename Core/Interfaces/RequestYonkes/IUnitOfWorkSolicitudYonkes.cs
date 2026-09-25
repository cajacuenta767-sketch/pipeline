using Core.Interfaces.RequestYonkes.CotizacionMessages;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces.RequestYonkes
{
	public interface IUnitOfWorkSolicitudYonkes : IDisposable
	{
		ISolicitudesYonkeRepository SolicitudYonkeRepository { get; }
		ISolicitudCotizacionRepository SolicitudCotizacionRepository { get; }
		ISolicitudCotizacionImagenesRepository solicitudCotizacionImagenesRepository { get; }

		ISolicitudCotizacionMensajeRepository SolicitudCotizacionMensajeRepository { get; }
		IOrdenRepository OrdenRepository { get; }
		IPagosRepository pagosRepository { get; }



		// 🔥 NUEVO
		Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

		void SaveChanges();

		Task SaveChangesAsync(CancellationToken cancellation);


	
	}
}
