using Core.DTO.Empresas;

namespace Core.Interfaces.Negocio.Calificacion
{
	public interface IYunkeCalificacionService
	{
		Task<Guid> RegistrarAsync(RegistrarCalificacionRequest request, CancellationToken cancellationToken);

		Task<CalificacionYonkeDto> ObtenerCalificacionAsync(Guid yonkeGuidId, CancellationToken cancellationToken);
	}
}
