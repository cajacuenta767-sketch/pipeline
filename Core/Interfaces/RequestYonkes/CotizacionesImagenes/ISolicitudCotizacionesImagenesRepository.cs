using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.CotizacionesImagenes
{
	public interface ISolicitudCotizacionesImagenesRepository
	{
		Task AgregarAsync(SolicitudCotizacionesImagenes imagen);
	}
}
