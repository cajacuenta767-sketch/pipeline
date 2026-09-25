namespace Core.Interfaces.Login.ClienteOtp
{
	public interface ISmsService
	{
		//Task<bool> EnviarSmsAsync(
		//	string telefono,
		//	string mensaje,
		//	CancellationToken cancellationToken = default);

		Task<bool> EnviarSmsAsync(
					string telefono,
					string mensaje,
					CancellationToken cancellationToken = default);


	}


}
