using Core.DTO.Login.AuthSoporte;
using Core.Interfaces.Login.AuthSoporte;
using Core.ResponseGlobal;
using Core.Services.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUsuarioService _usuarioService;

		public AuthController(IUsuarioService usuarioService)
		{
			_usuarioService = usuarioService;
		}


		[HttpPost("soporte")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte")]
		public async Task<IActionResult> CrearUsuarioSoporte([FromBody] CrearUsuarioSoporteDTO request, CancellationToken cancellationToken)
		{
			var usuarioId = await _usuarioService.CrearUsuarioSoporteAsync(
				request,
				cancellationToken);

			return Ok(
				ApiResponseHelper.Success(
					new
					{
						Id = usuarioId
					},
					"Usuario de soporte creado correctamente."));
		}



		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginSoporteDTO request, CancellationToken cancellationToken)
		{
			var resultado = await _usuarioService.LoginAsync(
				request,
				cancellationToken);

			return Ok(
				ApiResponseHelper.Success(
					resultado,
					"Login de soporte exitoso."));
		}
	}
}
