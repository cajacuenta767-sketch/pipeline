using Core.DTO.SolicitudMenssages;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.RequestYonkes.CotizacionesImagenes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Asociados
{
	[Route("api/[controller]")]
	[ApiController]
	public class SolicitudCotizacionMensajesController : ControllerBase
	{
		private readonly ISolicitudCotizacionMensajeService _service;
		public readonly ICurrentUserService _currentUserService;

		public SolicitudCotizacionMensajesController(ISolicitudCotizacionMensajeService service,
												     ICurrentUserService currentUserService)
		{
			_service = service;
			_currentUserService = currentUserService;
		}

		// ==========================================================
		// ENVIAR MENSAJE
		// ==========================================================

		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> EnviarMensaje([FromBody] RegistrarMensajeCotizacionRequest request, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			var resultado =
				await _service.EnviarMensajeAsync(
					request,
					usuarioId.Value,
					cancellationToken);

			return Ok(resultado);
		}


		// ==========================================================
		// OBTENER CONVERSACIÓN
		// ==========================================================

		[HttpGet("{solicitudCotizacionGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> ObtenerMensajes(Guid solicitudCotizacionGuidId, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			var mensajes =
				await _service.ObtenerMensajesAsync(
					solicitudCotizacionGuidId,
					usuarioId.Value,
					cancellationToken);

			return Ok(mensajes);
		}


		// ==========================================================
		// MARCAR MENSAJES COMO LEÍDOS
		// ==========================================================

		[HttpPut("{solicitudCotizacionGuidId:guid}/leer")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> MarcarComoLeidos(Guid solicitudCotizacionGuidId, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			await _service.MarcarComoLeidosAsync(
				solicitudCotizacionGuidId,
				usuarioId.Value,
				cancellationToken);

			return NoContent();
		}


		// ==========================================================
		// CANTIDAD DE MENSAJES NO LEÍDOS
		// ==========================================================

		[HttpGet("{solicitudCotizacionGuidId:guid}/no-leidos")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> ObtenerNoLeidos(Guid solicitudCotizacionGuidId, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			var cantidad =
				await _service.ObtenerCantidadNoLeidosAsync(
					solicitudCotizacionGuidId,
					usuarioId.Value,
					cancellationToken);

			return Ok(new
			{
				cantidad
			});
		}
	}
}
