using Core.DTO.SolicitudYonkes;
using Core.Entitys;

namespace Core.Interfaces.RequestYonkes
{
	public interface ISolicitudesYonkeRepository
	{
		/// <summary>
		/// Agrega un nuevo registro de envío de solicitud a un yonke.
		/// </summary>
		Task AgregarAsync(SolicitudYonkes entity);

		/// <summary>
		/// Agrega múltiples registros de envío en una sola operación.
		/// </summary>
		Task AgregarRangoAsync(IList<SolicitudYonkes> entities);

		/// <summary>
		/// Obtiene un registro por su Guid.
		/// </summary>
		Task<SolicitudYonkes?> ObtenerPorGuidAsync(Guid guidId);

		/// <summary>
		/// Utilizado para enviar notificacion en servicio de cotizaciones
		/// </summary>
		/// <param name="guidId"></param>
		/// <returns></returns>
		Task<SolicitudYonkes?> ObtenerConSolicitudPorGuidAsync(Guid guidId);

		Task<SolicitudYonkes?> GetDataSolicitudyonkeByGuidId(Guid guidId);

		/// <summary>
		/// Obtiene un registro por su Id.
		/// </summary>
		Task<SolicitudYonkes?> ObtenerPorIdAsync(int id);

		/// <summary>
		/// Obtiene el registro correspondiente a una solicitud y un yonke.
		/// </summary>
		Task<SolicitudYonkes?> ObtenerAsync(Guid SolicitudGuidId, Guid YonkeGuidId);

		/// <summary>
		/// Obtiene todos los yonkes a los que fue enviada una solicitud.
		/// </summary>
		Task<IList<SolicitudYonkes>> ObtenerPorSolicitudAsync(Guid SolicitudGuidId);

		/// <summary>
		/// Obtiene todas las solicitudes enviadas a un yonke.
		/// </summary>
		Task<IList<SolicitudYonkes>> ObtenerPorYonkeAsync(Guid YonkeGuidId);

		/// <summary>
		/// Verifica si una solicitud ya fue enviada a un yonke.
		/// </summary>
		Task<bool> ExisteEnvioAsync(Guid SolicitudGuidId, Guid YonkeGuidId);

		/// <summary>
		/// Actualiza un registro.
		/// </summary>
		Task ActualizarAsync(SolicitudYonkes entity);

		/// <summary>
		/// Elimina un registro.
		/// </summary>
		Task EliminarAsync(SolicitudYonkes entity);

		/// <summary>
		/// Obtener .
		/// </summary>
		Task<SolicitudYonkes?> ObtenerPorSolicitudGuidYonkeGuidAsync(Guid solicitudGuidId,	Guid yonkeGuidId);


		Task<IList<SolicitudYonkes>> ObtenerPendientesPorYonkeAsync(Guid YonkeGuidId);

		Task<int> ContarPendientesPorYonkeAsync(Guid YonkeGuidId);

		Task<IList<SolicitudYonkes>> ObtenerPorYonkeYEstatusAsync(Guid YonkeGuidId, int estatusId);

		//Contar los enviados para vista del cliente
		Task<int> ContarEnviosAsync(Guid SolicitudGuidId);


		//Contar solicitudes nuevas de cada yonke
		Task<int> ContarPorSolicitudYEstatusAsync(Guid SolicitudGuidId, int estatusId);


		//Mas reciente Solitciud de cada yonke
		Task<SolicitudYonke_List_DTO> SolicitudMasRecienteByYonke(Guid yonkeGuidId);

		//Todas las solicitudes de cada yonke 
		Task<List<SolicitudYonke_List_DTO>> ObtenerSolicitudesPorYonkeAsync(Guid yonkeGuidId, CancellationToken cancellationToken);
	}
}
