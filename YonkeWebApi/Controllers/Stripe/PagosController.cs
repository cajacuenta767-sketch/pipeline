using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Stripe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ApiYonke.Controllers.Stripe
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]

	public class PagosController : ControllerBase
	{
		private readonly IStripePaymentService _stripePaymentService;
		private readonly ICurrentUserService _currentUserService;

		public PagosController(IStripePaymentService stripePaymentService,
							   ICurrentUserService currentUserService)
		{
			_stripePaymentService = stripePaymentService;
			_currentUserService = currentUserService;
		}

		

		// ==========================================
		// CREAR CHECKOUT STRIPE
		// ==========================================

		//[Authorize]
		[HttpPost("checkout/{ordenGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> CrearCheckout(Guid ordenGuidId, CancellationToken cancellationToken = default)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No fue posible identificar al usuario.");
			}


			var result =
				await _stripePaymentService
					.CrearCheckoutAsync(
						ordenGuidId,
						usuarioId.Value,
						cancellationToken);


			return Ok(result);
		}



		// ==========================================
		// STRIPE WEBHOOK
		// ==========================================

		[AllowAnonymous]
		[HttpPost("stripe/webhook")]
		public async Task<IActionResult> StripeWebhook(	CancellationToken cancellationToken)
		{
			Console.WriteLine( "🔥🔥🔥 LLEGÓ AL CONTROLADOR STRIPE WEBHOOK 🔥🔥🔥");

			try
			{
				// ==========================================
				// 1. LEER BODY
				// ==========================================

				using var reader =
					new StreamReader(Request.Body);

				var json =
					await reader.ReadToEndAsync(
						cancellationToken);


				if (string.IsNullOrWhiteSpace(json))
				{
					return BadRequest(
						"El webhook está vacío.");
				}


				// ==========================================
				// 2. FIRMA
				// ==========================================

				var signature =
					Request.Headers["Stripe-Signature"]
						.FirstOrDefault();


				if (string.IsNullOrWhiteSpace(signature))
				{
					return BadRequest(
						"Falta Stripe-Signature.");
				}


				// ==========================================
				// 3. IDENTIFICAR EVENTO
				// ==========================================

				var stripeEvent =
					EventUtility.ParseEvent(
						json,
						throwOnApiVersionMismatch: false);


				Console.WriteLine($"========================================");

				Console.WriteLine($"STRIPE WEBHOOK RECIBIDO");
				Console.WriteLine($"Event ID: {stripeEvent.Id}");
				Console.WriteLine($"Event Type: {stripeEvent.Type}");

				Console.WriteLine($"========================================");


				// ==========================================
				// 4. PROCESAR WEBHOOK
				// ==========================================

				var procesado =
					await _stripePaymentService
						.ProcesarWebhookAsync(
							json,
							signature,
							cancellationToken);


				// ==========================================
				// 5. RESULTADO
				// ==========================================

				if (!procesado)
				{
					Console.WriteLine(
						"ProcesarWebhookAsync devolvió FALSE.");

					return BadRequest(
						"ProcesarWebhookAsync devolvió FALSE.");
				}


				Console.WriteLine(
					"Webhook procesado correctamente.");


				return Ok();
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(
					$"InvalidOperationException: {ex}");

				return BadRequest(
					$"Error de validación del webhook: {ex.Message}");
			}
			catch (StripeException ex)
			{
				Console.WriteLine(
					$"StripeException: {ex}");

				return StatusCode(
					StatusCodes.Status500InternalServerError,
					$"Error de Stripe: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Exception: {ex}");

				return StatusCode(
					StatusCodes.Status500InternalServerError,
					$"Error interno: {ex.Message}");
			}
		}



		[AllowAnonymous]
		[HttpGet("/pago/exitoso")]
		public async Task<IActionResult> PagoExitoso([FromQuery] string session_id)
		{
			if (string.IsNullOrWhiteSpace(session_id))
				return BadRequest("No se recibió session_id.");

			try
			{
				var service = new SessionService();

				var session = await service.GetAsync(session_id);

				return Ok(new
				{
					mensaje = "Pago realizado correctamente",
					sessionId = session.Id,
					paymentStatus = session.PaymentStatus,
					paymentIntentId = session.PaymentIntentId
				});
			}
			catch (StripeException ex)
			{
				return BadRequest(new
				{
					mensaje = "No se pudo consultar la sesión de Stripe.",
					error = ex.Message
				});
			}
		}



		//[Authorize]
		[HttpGet("resultado/{sessionId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<IActionResult> ObtenerResultadoPago(string sessionId,  CancellationToken cancellationToken)
		{
			try
			{
				var usuarioId = _currentUserService.UserId;

				if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
				{
					throw new UnauthorizedAccessException(
						"No fue posible identificar al usuario.");
				}

				var resultado =
					await _stripePaymentService
						.ObtenerResultadoPagoAsync(
							sessionId,
							usuarioId.Value,
							cancellationToken);

				return Ok(resultado);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new
				{
					mensaje = ex.Message
				});
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid();
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					new
					{
						mensaje = "Error al consultar el resultado del pago."
					});
			}
		}
	}

}
