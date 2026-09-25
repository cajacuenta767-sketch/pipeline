using Core.Entitys;

namespace Core.Interfaces.Login.ClienteOtp
{
	public interface IClienteOtpService
	{
		Task SolicitarOtpAsync(string telefono, string? ip, CancellationToken cancellationToken = default);

		Task<Clientes> VerificarOtpAsync(string telefono, string codigo, CancellationToken cancellationToken = default);
	}
}
