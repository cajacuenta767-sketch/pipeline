using Core.DTO.Login;
using Core.Exceptions;
using Core.Interfaces.JWT;
using Core.Interfaces.Login.Yonke;
using Core.Interfaces.Negocio;
using Microsoft.AspNetCore.Identity;

namespace Core.Services.Login
{
	public class YonkeAuthService : IYonkeAuthService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUnitOfWorkNegocio _unitOfWork;
		private readonly IJwtService _jwtService;


		public YonkeAuthService(UserManager<IdentityUser> userManager,
								IUnitOfWorkNegocio unitOfWork,
								IJwtService jwtService)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_jwtService = jwtService;
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
	}
}
