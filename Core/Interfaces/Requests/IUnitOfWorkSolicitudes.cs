using Core.Entitys;
using Core.Interfaces.Requests.historials;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces.Requests
{
	public interface IUnitOfWorkSolicitudes : IDisposable
	{
		ISolicitudesRepository<Solicitudes> SolicitudesRepository { get; }
		ISolicitudCiudadesRepository SolicitudCiudadesRepository { get; }
		ISolicitudesEstatusRepository<SolicitudesEstatus> solicitudesEstatusRepository { get; }
		ISolicitudImagenesRepository<SolicitudesImagenes> solicitudImagenesRepository { get; }
		ISolicitudHistorialRepository<SolicitudesHistorials> solicitudHistorialRepository { get; }

		// 🔥 NUEVO
		Task<IDbContextTransaction> BeginTransactionAsync();

		void SaveChanges();

		Task SaveChangesAsync(CancellationToken cancellationToken);

		Task SaveChangesAsync();
	}
}
