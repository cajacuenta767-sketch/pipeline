namespace Core.Interfaces.RequestYonkes.Solicitudes
{
	public interface ISolicitudYonkeService
	{
		Task<int> EnviarSolicitudAsync(Guid solicitudGuidId, CancellationToken cancellationToken);
		Task MarcarComoVistaAsync(Guid solicitudYonkeGuidId, CancellationToken cancellationToken);

		//Task RegistrarCotizacionAsync(Guid solicitudYonkeGuidId, RegistrarCotizacionRequest request, CancellationToken cancellation);
	}
}
