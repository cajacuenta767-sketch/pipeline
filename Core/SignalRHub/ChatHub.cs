using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Core.SignalRHub
{
	[Authorize]
	public class ChatHub : Hub
	{
		public async Task UnirseCotizacion(Guid cotizacionGuidId)
		{
			var usuarioId = Context.UserIdentifier;

			if (string.IsNullOrEmpty(usuarioId))
			{
				throw new HubException("Usuario no autenticado.");
			}

			var grupo = $"cotizacion-{cotizacionGuidId}";

			await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
		}

		public async Task SalirCotizacion(Guid cotizacionGuidId)
		{
			var grupo = $"cotizacion-{cotizacionGuidId}";

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
		}
	}
}
