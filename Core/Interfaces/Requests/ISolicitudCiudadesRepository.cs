using Core.DTO.Solicitudes.Citys;
using Core.Entitys;

namespace Core.Interfaces.Requests
{
	public interface ISolicitudCiudadesRepository
	{

		Task<IList<SolicitudesCiudades>> ObtenerPorSolicitudAsync(Guid solicitudGuidId);

		Task<CiudadesListBySolicitud_DTO?> ObtenerCiudadesPorSolicitudAsync(Guid solicitudGuidId);

		/// <summary>
		/// Agrega una ciudad asociada a una solicitud.
		/// </summary>
		Task AgregarAsync(SolicitudesCiudades entity);


		/// <summary>
		/// Agrega varias ciudades asociadas a una solicitud.
		/// </summary>
		Task AgregarRangoAsync(IList<SolicitudesCiudades> entities);

		/// <summary>
		/// Obtiene una relación específica solicitud-ciudad.
		/// </summary>
		Task<SolicitudesCiudades?> ObtenerAsync(Guid solicitudGuidId, int ciudadId);


		/// <summary>
		/// Verifica si una ciudad ya está asociada a una solicitud.
		/// </summary>
		Task<bool> ExisteAsync(Guid solicitudGuidId, int ciudadId);


		/// <summary>
		/// Elimina una ciudad de una solicitud.
		/// </summary>
		Task EliminarAsync(SolicitudesCiudades entity);


		/// <summary>
		/// Elimina todas las ciudades asociadas a una solicitud.
		/// </summary>
		Task EliminarPorSolicitudAsync(Guid solicitudGuidId);
	}
}
