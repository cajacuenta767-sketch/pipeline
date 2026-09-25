using Core.Entitys;

namespace Core.Interfaces.RequestYonkes
{
	public interface ISolicitudCotizacionRepository
	{
		Task AgregarAsync(SolicitudCotizaciones cotizacion);

		Task<SolicitudCotizaciones?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken);

		Task ActualizarAsync(SolicitudCotizaciones cotizacion);

		Task<bool> ExisteCotizacionAsync(Guid solicitudYonkeGuidId);
		//Task<SolicitudCotizaciones> ObtenerPorStripePaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
	}
}
