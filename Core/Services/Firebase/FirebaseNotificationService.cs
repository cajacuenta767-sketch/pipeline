using Core.Interfaces.Firebase;

namespace Core.Services.Firebase
{
	using FirebaseAdmin.Messaging;

	public class FirebaseNotificationService : IFirebaseNotificationService
	{
	

		public async Task SendMulticastAsync(IEnumerable<string> tokens, 
			string titulo, 
			string cuerpo, 
			Dictionary<string, string> data,
			CancellationToken cancellationToken)
		{
			var tokensList = tokens
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct()
				.ToList();

			if (!tokensList.Any())
			{
				return;
			}

			var message = new MulticastMessage
			{
				Tokens = tokensList,
				Notification = new Notification
				{
					Title = titulo,
					Body = cuerpo
				},
				Data = data
			};

			await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);
		}



		public async Task EnviarNotificacionAsync(
		   IEnumerable<string> tokens,
		   string titulo,
		   string cuerpo,
		   Dictionary<string, string>? data = null,
		   CancellationToken cancellationToken = default)
		{
			var tokensLista = tokens
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct()
				.ToList();

			if (!tokensLista.Any())
				return;

			var message = new MulticastMessage
			{
				Tokens = tokensLista,
				Notification = new Notification
				{
					Title = titulo,
					Body = cuerpo
				},

				Data = data ?? new Dictionary<string, string>()
			};

			await FirebaseMessaging.DefaultInstance
				.SendEachForMulticastAsync(message,	cancellationToken);
		}



	}
}
