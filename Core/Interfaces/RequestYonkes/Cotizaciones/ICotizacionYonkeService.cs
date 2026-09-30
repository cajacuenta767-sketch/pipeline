using Core.DTO.SolocitudCotizaciones;
using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.Cotizaciones
{
	public interface ICotizacionYonkeService
	{
		/// <summary>
		/// Registra una cotización para una solicitud enviada a un yonke.
		/// </summary>
		Task<Guid> RegistrarCotizacionAsync(Guid solicitudYonkeGuidId, 
											RegistrarCotizacionRequest request,
											CancellationToken cancellationToken);

		/// <summary>
		/// Obtiene una cotización por su Guid.
		/// </summary>
		Task<SolicitudCotizaciones?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken);

		/// <summary>
		/// Actualiza una cotización existente.
		/// </summary>
		//Task ActualizarCotizacionAsync(Guid cotizacionGuidId,
		//							   RegistrarCotizacionRequest request,
		//							   Guid usuarioId,
		//							   CancellationToken cancellationToken);

		Task ActualizarCotizacionAsync(Guid cotizacionGuidId,
									   RegistrarCotizacionRequest request,
									   CancellationToken cancellationToken);


		//Total de cotizciones por cada yonke auntenticado
		Task<int> ObtenerCotizacionesPorYonkeAsync(CancellationToken cancellationToken);
	}
}
