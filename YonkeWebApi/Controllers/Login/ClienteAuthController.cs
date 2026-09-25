using Core.DTO.Google_Login;
using Core.DTO.Login;
using Core.DTO.Login.otp;
using Core.DTO.Login.usuarioDispo;
using Core.Interfaces.Login.UserDispotivos;
using Core.Interfaces.Login_Cliente.GoogleApple;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiYonke.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClienteAuthController : ControllerBase
	{
		private readonly IClienteAuthService _clienteAuthService;
		private readonly IUsuariosDispositivosService _usuariosDispositivosService;

		public ClienteAuthController(IClienteAuthService clienteAuthService,
									 IUsuariosDispositivosService usuariosDispositivosRepository)
		{
			_clienteAuthService = clienteAuthService;
			_usuariosDispositivosService = usuariosDispositivosRepository;
		}



		#region No usado google
		/// <summary>
		/// Login Google cuenta
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		//[HttpPost("google")]
		//[AllowAnonymous]
		//public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest request)
		//{
		//	var respuesta = await _clienteAuthService.LoginGoogleAsync(request);

		//	return Ok(respuesta);
		//}


		//[HttpPost("apple")]
		//[AllowAnonymous]
		//public async Task<IActionResult> LoginApple([FromBody] AppleLoginRequest request)
		//{
		//	var respuesta = await _clienteAuthService.LoginAppleAsync(request);

		//	return Ok(respuesta);
		//}
		#endregion



		// ============================================================
		// GOOGLE LOGIN
		// ============================================================

		[HttpPost("google")]
		public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest request)
		{
			try
			{
				var response = await _clienteAuthService.LoginGoogleAsync(request);

				return Ok(response);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "Error al iniciar sesión con Google."
				});
			}
		}
    
		








		[AllowAnonymous]
		[HttpPost("solicitar-otp")]
		public async Task<IActionResult> SolicitarOtp([FromBody] SolicitarOtpRequest request, CancellationToken cancellationToken)
		{
			try
			{
				if (request == null)
				{
					return BadRequest(new
					{
						success = false,
						message = "La solicitud es obligatoria."
					});
				}

				var ip =
					HttpContext.Connection
						.RemoteIpAddress?
						.ToString();

				await _clienteAuthService.SolicitarOtpAsync(
					request,
					ip,
					cancellationToken);

				return Ok(new
				{
					success = true,
					message =
						"Se envió un código de verificación a tu teléfono."
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (Exception ex)
			{
				// Registrar ex con ILogger
				return StatusCode(500, new
				{
					success = false,
					message = "Ocurrió un error al procesar la solicitud."
				});
			}
		}


		[AllowAnonymous]
		[HttpPost("verificar-otp")]
		public async Task<IActionResult> VerificarOtp([FromBody] VerificarOtpRequest request, CancellationToken cancellationToken = default)
		{
			try
			{
				var resultado =
					await _clienteAuthService.VerificarOtpAsync(
						request,
						cancellationToken);

				return Ok(resultado);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new
				{
					success = false,
					message = ex.Message
				});
			}
		}




		/// <summary>
		/// Regisstro del dispotivo del cleinte despues de hacer login exitoso
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpPost("registrar-dispositivo")]
		[Authorize]
		public async Task<IActionResult> RegistrarDispositivo([FromBody] RegistrarDispositivoRequest request, CancellationToken cancellationToken = default)
		{
			try
			{
				if (request == null)
				{
					return BadRequest(new
					{
						success = false,
						message = "La solicitud es obligatoria."
					});
				}

				var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(usuarioId))
				{
					return Unauthorized(new
					{
						success = false,
						message = "No se pudo identificar al usuario."
					});
				}

				var resultado =
					await _usuariosDispositivosService.RegistrarDispositivoAsync(
						usuarioId,
						request,
						cancellationToken);

				return Ok(new
				{
					success = resultado,
					message = "Dispositivo registrado correctamente."
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "Ocurrió un error al registrar el dispositivo."
				});
			}
		}

	}
}
