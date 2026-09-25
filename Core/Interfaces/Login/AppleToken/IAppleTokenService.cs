using Core.DTO.Login;

namespace Core.Interfaces.Login.AppleToken
{
	public interface IAppleTokenService
	{
		Task<AppleTokenPayload> ValidateAsync(string identityToken);

	}
}
