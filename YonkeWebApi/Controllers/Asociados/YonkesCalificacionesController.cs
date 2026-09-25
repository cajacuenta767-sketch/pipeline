using Core.DTO.Empresas;
using Core.Interfaces.Negocio.Calificacion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Asociados
{
	[Route("api/[controller]")]
	[ApiController]
	public class YonkesCalificacionesController : ControllerBase
	{
		private readonly IYunkeCalificacionService _service;

		public YonkesCalificacionesController(IYunkeCalificacionService service)
		{
			_service = service;
		}

		// ==========================================
		// POST: api/yonkes/calificaciones
		// ==========================================

		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> Registrar([FromBody] RegistrarCalificacionRequest request, CancellationToken cancellationToken)
		{
			var guidId = await _service.RegistrarAsync(
				request,
				cancellationToken);

			return Ok(new
			{
				success = true,
				message = "La calificación fue registrada correctamente.",
				guidId
			});
		}


		// ==========================================
		// GET:
		// api/yonkes/calificaciones/{yonkeGuidId}
		// ==========================================

		[HttpGet("{yonkeGuidId:guid}")]
		[AllowAnonymous]
		public async Task<IActionResult> Obtener(Guid yonkeGuidId, CancellationToken cancellationToken)
		{
			var resultado =
				await _service.ObtenerCalificacionAsync(
					yonkeGuidId,
					cancellationToken);

			return Ok(resultado);
		}
	}
}
