namespace Core.Interfaces.Firebase
{
	public interface IFirebaseNotificationService
	{
		Task SendMulticastAsync(
			IEnumerable<string> tokens,
			string titulo,
			string cuerpo,
			Dictionary<string, string> data,
			CancellationToken cancellationToken);


		Task EnviarNotificacionAsync(
			 IEnumerable<string> tokens,
			 string titulo,
			 string cuerpo,
			 Dictionary<string, string>? data = null,
			 CancellationToken cancellationToken = default);
	}


}
