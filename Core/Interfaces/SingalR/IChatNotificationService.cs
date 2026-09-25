namespace Core.Interfaces.SingalR
{
	public interface IChatNotificationService
	{
		
		Task EnviarMensajeAsync(
			Guid solicitudCotizacionGuidId,
			object mensaje,
			CancellationToken cancellationToken = default);


	}
}
