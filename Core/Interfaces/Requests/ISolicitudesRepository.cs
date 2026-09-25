using Core.DTO.Solicitudes.Requests;
using Core.DTO.SolocitudCotizaciones;
using Core.EntityBase;
using Core.Entitys;
using System.Linq.Expressions;

namespace Core.Interfaces.Requests
{
	public interface ISolicitudesRepository<T> where T : BaseEntity
	{
		IQueryable<Solicitud_Busqueda_DTO> GetSolicitudesByUserId(DateTime desde, DateTime hasta, Guid userId);
		Task<Solicitud_Busqueda_DTO> GetSolicitudByGuidId(Guid guidId );
		Task<Solicitudes?> GetByGuidId(Guid guidId);
		Task AddAsync(T entity, CancellationToken cancellationToken);
		void UpdateSolicitud(Solicitudes solicitudUpdateDTO);
		void BajaSolicitud(Guid guidId);




		// NUEVO
		Task<string> GenerarFolioSolicitudAsync();

		//Contar para dashboard del cliente 
		Task<int> ContarSolicitudesPorUsuarioAsync(Guid usuarioId);
		Task<int> ContarCotizacionesPorUsuarioAsync(Guid usuarioId);


		// Ver mis solicitudes by User Dashboard
		IQueryable<Solicitud_Busqueda_DTO> GetMisSolicitudesByUserDashboard(Guid userId);

		IQueryable<Cotizacion_List_Dashboad_DTO> GetCotizacionesByUserId(Guid userId);




		//Solicitud mas reciente
		Task<Solicitud_Busqueda_DTO?> ObtenerSolicitudMasRecienteAsync(Guid userId);


		//Contar para saber los limites diarios de usaurios freee
		Task<int> CountAsync(Expression<Func<Solicitudes, bool>> predicate, CancellationToken cancellationToken = default);
	}
}
