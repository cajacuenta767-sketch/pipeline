namespace Core.Services.SignalR
{
	using Core.Interfaces.SingalR;
	using Core.SignalRHub;
	using Microsoft.AspNetCore.SignalR;

	public class SignalRNotificationService	: IChatNotificationService
	{
		private readonly IHubContext<ChatHub> _hubContext;

		public SignalRNotificationService(IHubContext<ChatHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public async Task EnviarMensajeAsync(
		Guid solicitudCotizacionGuidId,
		object mensaje,
		CancellationToken cancellationToken = default)
		{
			var grupo = $"cotizacion-{solicitudCotizacionGuidId}";

			await _hubContext.Clients
				.Group(grupo)
				.SendAsync(
					"NuevoMensaje",
					mensaje,
					cancellationToken);
		}


		//Probar que funcione
		public async Task NotificarYonkeAsync(Guid yonkeGuidId, object mensaje)
		{
			await _hubContext.Clients
				.Group(yonkeGuidId.ToString())
				.SendAsync(
					"NuevaSolicitud",
					mensaje);
		}



	}
}
