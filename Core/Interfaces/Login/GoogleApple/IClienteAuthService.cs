using Core.DTO.Google_Login;
using Core.DTO.Login;
using Core.DTO.Login.otp;

namespace Core.Interfaces.Login_Cliente.GoogleApple
{
	public interface IClienteAuthService
	{
		Task<LoginClienteResponse> LoginGoogleAsync(GoogleLoginRequest request);

		Task<LoginClienteResponse> LoginAppleAsync(AppleLoginRequest request);


		Task SolicitarOtpAsync(SolicitarOtpRequest request, string? ip, CancellationToken cancellationToken = default);

		Task<LoginClienteResponse> VerificarOtpAsync(VerificarOtpRequest request, CancellationToken cancellationToken = default);

		Task<ClientePerfilDTO?> ObtenerPerfilPublicoAsync(Guid clienteGuidId);
	}
}
