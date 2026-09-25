using Core.Models.BuildSecurity;

namespace Core.Interfaces.BuildSecurity
{
	public interface IUserService
	{

		Task<UserManagerResponse> RegisterAsociadoAsync(RegisterViewAspociadoModel model);






		//Task<UserManagerResponse> RegisterUserAsync(RegisterViewUsersModel model);
		//Task<UserManagerResponse> RegisterSoporteAsync(RegisterViewSoporteModel model);

		//Task<UserManagerResponse> RegisterClienteAsync(RegistroClienteModel model);

		//Task<UserManagerResponse> RegisterTrabajadorAsync(RegistroClienteModel model);

		////Task<UserTokenSoporteResponse> LoginSoporteAsync(LoginViewModel model);

		//Task<UserTokenResponse> LoginUserAsync(LoginViewModel model);

		//Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

		//Task<UserManagerResponse> ForgetPasswordAsync(string email);

		//Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);

		//Task<UserManagerResponse> ReEnvioConfirmCuentaAsync(ReenviarConfirmEmailViewModel model);
	}
}
