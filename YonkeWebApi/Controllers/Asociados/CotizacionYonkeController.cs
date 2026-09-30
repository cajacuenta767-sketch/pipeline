using Core.DTO.SolocitudCotizaciones;
using Core.Entitys;
using Core.Interfaces.RequestYonkes.Cotizaciones;
using Core.ResponseGlobal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiYonke.Controllers.Asociados
{
	[ApiController]
	[Route("api/[controller]")]
	
	public class CotizacionYonkeController : ControllerBase
	{
		private readonly ICotizacionYonkeService _cotizacionYonkeService;

		public CotizacionYonkeController(ICotizacionYonkeService cotizacionYonkeService)
		{
			_cotizacionYonkeService = cotizacionYonkeService;
		}


		/// <summary>
		/// Registra una nueva cotización.
		/// </summary>
		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<IActionResult> RegistrarCotizacion(Guid solicitudYonkeGuidId,	[FromForm] RegistrarCotizacionRequest request, CancellationToken cancellationToken)
		{	
			var guid = await _cotizacionYonkeService.RegistrarCotizacionAsync(
				solicitudYonkeGuidId,
				request,
				cancellationToken);

			return Ok(ApiResponseGlobal<Guid>.Ok(
				guid,
				"Cotización registrada correctamente."));
		}



		/// <summary>
		/// Obtiene una cotización por Guid.
		/// </summary>
		[HttpGet("{cotizacionGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente")]
		public async Task<ActionResult<ApiResponseGlobal<SolicitudCotizaciones>>> ObtenerCotizacion(Guid cotizacionGuidId, CancellationToken cancellationToken)
		{
			var cotizacion = await _cotizacionYonkeService
				.ObtenerPorGuidAsync(cotizacionGuidId, cancellationToken);

			if (cotizacion == null)
				return NotFound(ApiResponseGlobal<string>.Fail("La cotización no existe."));

			return Ok(ApiResponseGlobal<SolicitudCotizaciones>.Ok(
				cotizacion,
				"Cotización obtenida correctamente."));
		}



		/// <summary>
		/// Actualiza una cotización.
		/// </summary>
		[HttpPut("{cotizacionGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<ActionResult<ApiResponseGlobal<string>>> ActualizarCotizacion(Guid cotizacionGuidId, [FromBody] RegistrarCotizacionRequest request, CancellationToken cancellationToken)
		{
			await _cotizacionYonkeService.ActualizarCotizacionAsync(
				cotizacionGuidId,
				request,
				cancellationToken);

			return Ok(ApiResponseGlobal<string>.Ok(
				string.Empty,
				"Cotización actualizada correctamente."));
		}



		////Cotizaciones enviadas////
		[HttpGet("MisCotizaciones/Total")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<ActionResult<ApiResponseGlobal<int>>> ObtenerTotalCotizaciones(CancellationToken cancellationToken)
		{
			var total = await _cotizacionYonkeService
				.ObtenerCotizacionesPorYonkeAsync(cancellationToken);

			return Ok(
				ApiResponseGlobal<int>.Ok(
					total,
					"Total de cotizaciones obtenido correctamente."));
		}



	}
}
