using Core.DTO.Login;

namespace Core.Interfaces.Login.Yonke
{
	public interface IYonkeAuthService
	{
		Task<LoginYonkeResponse> LoginAsync(LoginYonkeRequest request);
	}
}
