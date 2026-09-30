using Core.DTO.Login;
using Core.Models.BuildSecurity;

namespace Core.Interfaces.Login.Yonke
{
	public interface IYonkeAuthService
	{
		Task<LoginYonkeResponse> LoginAsync(LoginYonkeRequest request);

		Task<UserManagerResponse> ForgetPasswordAsync(string email);

		Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
	}
}
