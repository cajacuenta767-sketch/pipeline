using Core.DTO.Solicitudes.Citys;
using Core.Entitys;

namespace Core.Interfaces.Requests.ciudades
{
	public interface ISolicitudesCiudadesRepositorio
	{
		Task AgregarAsync(SolicitudesCiudades entity);

		Task AgregarRangoAsync(IList<SolicitudesCiudades> entities);

		Task<IEnumerable<SolicitudesCiudades>> ObtenerCiudadesPorSolicitudAsync(Guid solicitudGuidId);

		Task<IList<CiudadesListBySolicitud_DTO>> CiudadesBySolicitudsync(Guid solicitudGuidId);

		Task EliminarAsync(SolicitudesCiudades entity);

		Task EliminarPorSolicitudAsync(Guid solicitudGuidId);
	}
}
