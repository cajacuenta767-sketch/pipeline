using Core.DTO.Login;
using Core.Exceptions;
using Core.Interfaces.JWT;
using Core.Interfaces.Login.Yonke;
using Core.Interfaces.Negocio;
using Core.Interfaces.SmtpGmail;
using Core.Models.BuildSecurity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Core.Services.Login
{
	public class YonkeAuthService : IYonkeAuthService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUnitOfWorkNegocio _unitOfWork;
		private readonly IJwtService _jwtService;
		private readonly IMailSmtpService _mailService;
		private IConfiguration _configuration;


		public YonkeAuthService(UserManager<IdentityUser> userManager,
								IUnitOfWorkNegocio unitOfWork,
								IJwtService jwtService,
								IMailSmtpService mailSmtpService,
								IConfiguration configuration)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_jwtService = jwtService;
			_mailService = mailSmtpService;
			_configuration = configuration;
		}

		

		public async Task<LoginYonkeResponse> LoginAsync(LoginYonkeRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Correo))
				throw new BusinessException("Debe proporcionar un correo electrónico.");

			if (string.IsNullOrWhiteSpace(request.Password))
				throw new BusinessException("Debe proporcionar una contraseña.");

			// Buscar usuario de Identity
			var user = await _userManager.FindByEmailAsync(request.Correo);

			if (user == null)
				throw new BusinessException("Correo o contraseña incorrectos.");

			// Validar si el usuario está bloqueado
			if (await _userManager.IsLockedOutAsync(user))
				throw new BusinessException("La cuenta se encuentra bloqueada temporalmente.");

			// Validar confirmación de correo
			if (!user.EmailConfirmed)
				throw new BusinessException("Debe confirmar su correo electrónico antes de iniciar sesión.");

			var resultado = _userManager.PasswordHasher.VerifyHashedPassword(
			user,
			user.PasswordHash!,
			request.Password);
			Console.WriteLine(resultado);

			// Validar contraseña
			var passwordValido = await _userManager.CheckPasswordAsync(user, request.Password);

			if (!passwordValido)
			{
				await _userManager.AccessFailedAsync(user);
				throw new BusinessException("Correo o contraseña incorrectos.");
			}

			// Reiniciar contador de intentos fallidos
			await _userManager.ResetAccessFailedCountAsync(user);

			// Obtener información del yonke
			var yonke = await _unitOfWork.YunkeRepository.ObtenerPorCorreoAsync(request.Correo.TrimEnd());

			if (yonke == null)
				throw new BusinessException("No existe información del yonke.");

			if (!yonke.Estatus)
				throw new BusinessException("El yonke se encuentra inactiva.");

			if (!yonke.Autorizado == false)
				throw new BusinessException("La cuenta del yonke no esta autorizado.");

			// Obtener roles de Identity
			var roles = await _userManager.GetRolesAsync(user);

			// Generar JWT
			var token = _jwtService.GenerarTokenYunke(
				yonke,
				user,
				roles);

			return new LoginYonkeResponse
			{
				Token = token,
				YonkeGuidId = yonke.GuidId,
				Nombre = yonke.Nombre,
				Correo = user.Email ?? string.Empty
			};
		}


		public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message =
						"Debes ingresar un correo electrónico válido."
				};
			}

			email = email.Trim();


			// =========================================================
			// Buscar usuario
			// =========================================================

			var user =
				await _userManager.FindByEmailAsync(email);


			// =========================================================
			// No revelar si existe
			// =========================================================

			if (user == null)
			{
				return new UserManagerResponse
				{
					IsSuccess = true,
					Message =
						"Si el correo existe, recibirás instrucciones para restablecer tu contraseña."
				};
			}


			// =========================================================
			// Generar token Identity
			// =========================================================

			var token =
				await _userManager.GeneratePasswordResetTokenAsync(user);


			// =========================================================
			// Codificar token
			// =========================================================

			var encodedToken =
				WebEncoders.Base64UrlEncode(
					Encoding.UTF8.GetBytes(token));


			// =========================================================
			// URL DE LA API
			// =========================================================

			var apiBaseUrl =
				_configuration["Api:BaseUrl"];

			if (string.IsNullOrWhiteSpace(apiBaseUrl))
			{
				throw new BusinessException(
					"No está configurada la URL base de la API.");
			}


			var resetUrl =
				$"{apiBaseUrl.TrimEnd('/')}" +
				$"/api/YonkeAuth/reset-password-page" +
				$"?email={Uri.EscapeDataString(email)}" +
				$"&token={Uri.EscapeDataString(encodedToken)}";


			// =========================================================
			// Nombre
			// =========================================================

			var nombre =
				System.Net.WebUtility.HtmlEncode(
					user.UserName ?? email);


			var safeResetUrl =
				System.Net.WebUtility.HtmlEncode(
					resetUrl);


			// =========================================================
			// HTML EMAIL
			// =========================================================

			var body = $@"
			<table width='100%'
				   style='font-family:Arial;background:#f4f6f8;padding:20px'>

				<tr>

					<td align='center'>

						<table width='500'
							   style='background:white;
									  padding:30px;
									  border-radius:8px'>

							<tr>

								<td align='center'>

									<h2 style='color:#2563eb'>
										RefaNet
									</h2>

									<h3>
										Restablecer contraseña
									</h3>

								</td>

							</tr>

							<tr>

								<td>

									<p>
										Hola <b>{nombre}</b>,
									</p>

									<p>
										Recibimos una solicitud para
										restablecer la contraseña de
										tu cuenta de RefaNet.
									</p>

									<p align='center'>

										<a href='{safeResetUrl}'
										   style='background:#2563eb;
												  color:white;
												  padding:12px 25px;
												  text-decoration:none;
												  border-radius:6px;
												  display:inline-block'>

											Restablecer contraseña

										</a>

									</p>

									<p>
										Este enlace es válido durante
										<b>1 hora</b>.
									</p>

									<p style='color:#666'>

										Si tú no solicitaste este cambio,
										puedes ignorar este correo.

									</p>

									<hr>

									<small style='color:#999'>
										Equipo RefaNet
									</small>

								</td>

							</tr>

						</table>

					</td>

				</tr>

			</table>
			";


			// =========================================================
			// ENVIAR CORREO
			// =========================================================

			try
			{
				await _mailService.SendEmailGmailSmtpAsync(
					new List<string> { email },
					"Restablecer contraseña - RefaNet",
					body,
					[],
					""
				);
			}
			catch
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message =
						"No fue posible enviar el correo de recuperación."
				};
			}


			return new UserManagerResponse
			{
				IsSuccess = true,
				Message =
					"Si el correo existe, recibirás instrucciones para restablecer tu contraseña."
			};
		}


		public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
		{
			if (model == null)
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "La solicitud no es válida."
				};
			}


			if (string.IsNullOrWhiteSpace(model.Email))
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "El correo electrónico es obligatorio."
				};
			}


			if (string.IsNullOrWhiteSpace(model.Token))
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "El token de recuperación es obligatorio."
				};
			}


			if (string.IsNullOrWhiteSpace(model.NewPassword))
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "La nueva contraseña es obligatoria."
				};
			}


			if (model.NewPassword != model.ConfirmPassword)
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "Las contraseñas no coinciden."
				};
			}


			var email = model.Email.Trim();


			// =========================================================
			// Buscar usuario
			// =========================================================

			var user =
				await _userManager.FindByEmailAsync(email);


			if (user == null)
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message =
						"El enlace de recuperación no es válido."
				};
			}


			// =========================================================
			// Decodificar token
			// =========================================================

			string normalToken;

			try
			{
				var decodedToken =
					WebEncoders.Base64UrlDecode(
						model.Token);

				normalToken =
					Encoding.UTF8.GetString(
						decodedToken);
			}
			catch
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message =
						"El enlace de recuperación no es válido."
				};
			}


			// =========================================================
			// Cambiar contraseña
			// =========================================================

			var result =
				await _userManager.ResetPasswordAsync(
					user,
					normalToken,
					model.NewPassword);


			if (result.Succeeded)
			{
				return new UserManagerResponse
				{
					IsSuccess = true,
					Message =
						"La contraseña se actualizó correctamente."
				};
			}


			return new UserManagerResponse
			{
				IsSuccess = false,

				Message =
					"No fue posible actualizar la contraseña.",

				Errors =
					result.Errors.Select(
						e => e.Description)
			};
		}
	}
	
}
