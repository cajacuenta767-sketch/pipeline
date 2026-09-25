using Core.Entitys;

namespace Core.Interfaces.RequestYonkes
{
	public interface ISolicitudCotizacionImagenesRepository
	{
		Task AgregarAsync(SolicitudCotizacionesImagenes imagen);
	}
}
