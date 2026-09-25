using Core.DTO.Solicitudes.Citys;
using Core.Entitys;

namespace Core.Interfaces.Requests.ciudades
{
	public interface ISolicitudCiudadesService
	{
		Task AgregarAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken);

		Task AgregarRangoAsync(Guid solicitudGuidId, IList<int> ciudadesIds, CancellationToken cancellationToken);

		Task<IList<SolicitudesCiudades>> CiudadesBySolicitudsync(Guid solicitudGuidId);

		Task<CiudadesListBySolicitud_DTO> ObtenerCiudadesPorSolicitudAsync(Guid solicitudGuidId);

		Task<bool> ExisteAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken);

		Task ActualizarAsync(Guid solicitudGuidId, IList<int> ciudadesIds, CancellationToken cancellationToken);


	
		Task<bool> EliminarAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken);

		Task<bool> EliminarTodasAsync(Guid solicitudGuidId, CancellationToken cancellationToken);
	}
}
