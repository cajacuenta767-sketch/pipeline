using Core.Entitys;
using Core.Interfaces.RequestYonkes;
using Infra.DataContext;

namespace Infra.Repositorys.RequestYonkes
{
	public class CotizacionImagenesRepository : ISolicitudCotizacionImagenesRepository
	{
		private readonly AplicationDBContext _context;
		public CotizacionImagenesRepository(AplicationDBContext context)
		{
			_context = context;
		}

		public async Task AgregarAsync(SolicitudCotizacionesImagenes imagen)
		{
			await _context.SolicitudCotizacionesImagenes.AddAsync(imagen);
		}
	}
}
