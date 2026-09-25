using Core.DTO.OrdnesPago;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.RequestYonkes.OrdenesPago;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiYonke.Controllers.Stripe
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]
	public class OrdenController : ControllerBase
	{
		private readonly IOrdenService _ordenService;
		private readonly ICurrentUserService _currentUserService;

		public OrdenController(IOrdenService ordenService,
							   ICurrentUserService currentUserService)
		{
			_ordenService = ordenService;
			_currentUserService = currentUserService;
		}


		// ==========================================
		// OBTENER ORDEN
		// ==========================================

		[HttpGet("{guidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente, Soporte")]
		public async Task<IActionResult> Obtener(Guid guidId, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}

			var orden =
				await _ordenService
					.ObtenerPorGuidAsync(
						guidId,
						usuarioId.Value,
						cancellationToken);


			if (orden == null)
			{
				return NotFound(
					new
					{
						mensaje =
							"La orden no existe."
					});
			}


			return Ok(orden);
		}


		// ==========================================
		// OBTENER ORDEN POR COTIZACIÓN
		// ==========================================

		[HttpGet("cotizacion/{cotizacionGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> ObtenerPorCotizacion(Guid cotizacionGuidId, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}


			var orden =
				await _ordenService
					.ObtenerPorCotizacionAsync(
						cotizacionGuidId,
						usuarioId.Value,
						cancellationToken);


			if (orden == null)
			{
				return NotFound(
					new
					{
						mensaje =
							"Todavía no existe una orden para esta cotización."
					});
			}


			return Ok(orden);
		}


		// ==========================================
		// CREAR ORDEN
		// ==========================================

		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> Crear([FromBody] CrearOrdenRequest request, CancellationToken cancellationToken)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}


			var orden =
				await _ordenService
					.CrearDesdeCotizacionAsync(
						request.CotizacionGuidId,
						usuarioId.Value,
						cancellationToken);


			return Ok(
				new
				{
					mensaje = "Orden creada correctamente.",
					orden
				});
		}


		// ==========================================
		// CANCELAR ORDEN
		// ==========================================

		[HttpPost("{guidId:guid}/cancelar")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> Cancelar(Guid guidId, CancellationToken cancellationToken) 
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}


			var orden =
				await _ordenService
					.ActualizarEstatusAsync(
						guidId,
						usuarioId.Value,
						9, // Cancelada
						cancellationToken);


			return Ok(
				new
				{
					mensaje =
						"Orden cancelada correctamente.",

					orden
				});
		}
	}
}
