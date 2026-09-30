using Core.Entitys;

namespace Core.Interfaces.RequestYonkes
{
	public interface ISolicitudCotizacionRepository
	{
		Task AgregarAsync(SolicitudCotizaciones cotizacion);

		Task<SolicitudCotizaciones?> ObtenerPorGuidAsync(Guid guidId, CancellationToken cancellationToken);

		Task ActualizarAsync(SolicitudCotizaciones cotizacion);

		Task<bool> ExisteCotizacionAsync(Guid solicitudYonkeGuidId);


		//Total de cotizciones por cada yonke auntenticado
		Task<int> ContarPorYonkeAsync(Guid yonkeGuidId, CancellationToken cancellationToken);
	}
}
