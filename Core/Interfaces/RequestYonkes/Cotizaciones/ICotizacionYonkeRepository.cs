using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.Cotizaciones
{
	public interface ICotizacionYonkeRepository
	{
		/// <summary>
		/// Obtiene la cotización asociada a una solicitud enviada al yonke.
		/// </summary>
		Task<SolicitudCotizaciones?> ObtenerPorSolicitudYonkeGuidAsync(Guid solicitudYonkeGuidId);

		/// <summary>
		/// Valida si ya existe una cotización para esa solicitud.
		/// </summary>
		Task<bool> ExisteCotizacionAsync(Guid solicitudYonkeGuidId);

		
		//Contar las cotizaciones generadasd de cada Yonke Autenticad
		Task<int> ObtenerCotizacionesPorYonkeAsync(CancellationToken cancellationToken);
	}
}
