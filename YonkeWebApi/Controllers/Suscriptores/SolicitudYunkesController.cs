using Core.DTO.SolicitudYonkes;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.RequestYonkes.Solicitudes;
using Core.ResponseGlobal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Suscriptores
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]
	public class SolicitudYonkesController : ControllerBase
	{
		private readonly ISolicitudYonkeService _solicitudYonkeService;
		private readonly ICurrentUserService _currentUserService;

		public SolicitudYonkesController(ISolicitudYonkeService solicitudYonkeService,
										 ICurrentUserService currentUserService)
		{
			_solicitudYonkeService = solicitudYonkeService;
			_currentUserService = currentUserService;
		}


		/// <summary>
		/// Envía una solicitud a todos los yonkes con cobertura.
		/// </summary>
		[HttpPost("{solicitudGuidId}/enviar")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> EnviarSolicitud(Guid solicitudGuidId, CancellationToken cancellationToken)
		{
			var totalYonkes = await _solicitudYonkeService.EnviarSolicitudAsync(
				solicitudGuidId,
				cancellationToken);

			return Ok(ApiResponseGlobal<object>.Ok(
				new
				{
					SolicitudGuidId = solicitudGuidId,
					TotalYonkesEnviados = totalYonkes
				},
				$"La solicitud fue enviada correctamente a {totalYonkes} yonkes."));
		}

		/// <summary>
		/// Obtiene los yonkes a los que fue enviada una solicitud (destinatarios con cobertura).
		/// </summary>
		[HttpGet("solicitud/{solicitudGuidId:guid}/destinatarios")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Soporte")]
		public async Task<IActionResult> ObtenerDestinatariosPorSolicitud(Guid solicitudGuidId, CancellationToken cancellationToken)
		{
			var destinatarios = await _solicitudYonkeService
				.ObtenerDestinatariosPorSolicitudAsync(solicitudGuidId, cancellationToken);

			return Ok(ApiResponseGlobal<List<SolicitudYonkeDestinatarioDTO>>.Ok(
				destinatarios,
				"Destinatarios obtenidos correctamente."));
		}

		/// <summary>
		/// Marcar Como vista una solicitud por parte del Yunke
		/// </summary>
		/// <param name="solicitudYonkeGuidId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		//[Authorize(Roles = "Asociado")]
		[HttpPut("{solicitudYonkeGuidId:guid}/vista")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<IActionResult> MarcarComoVista(Guid solicitudYonkeGuidId, CancellationToken cancellationToken)
		{
			await _solicitudYonkeService.MarcarComoVistaAsync(solicitudYonkeGuidId, cancellationToken);

			return Ok(ApiResponseGlobal<string>.Ok(
				"OK",
				"La solicitud fue marcada como vista."));
		}




		//Total de Solicituides Nuevas de cada Yonke
		[HttpGet("TotalSolicitudesNuevas")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<IActionResult> ContarSolicitudesByYonke(CancellationToken cancellationToken)
		{
			var yonkeGuidId = _currentUserService.YonkeGuidId;

			if (!yonkeGuidId.HasValue || yonkeGuidId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar el Yonke del usuario autenticado.");
			}

			var solicitudes =
				await _solicitudYonkeService
					.ContarPendientesPorYonkeAsync(
					yonkeGuidId.Value);

			return Ok(
				ApiResponseHelper.Success(
					solicitudes,
					"Solicitudes nuevas obtenidas."));
		}



		//Mas reciente solicitud de cada yonke
		[HttpGet("MasReciente")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<IActionResult> SolicitudMasRecienteByYonke(CancellationToken cancellationToken)
		{
			var yonkeGuidId = _currentUserService.YonkeGuidId;

			if (!yonkeGuidId.HasValue || yonkeGuidId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar el Yonke del usuario autenticado.");
			}

			var solicitudes =
				await _solicitudYonkeService
					.SolicitudRecienteByYonke(
					yonkeGuidId.Value);

			return Ok(
				ApiResponseHelper.Success(
					solicitudes,
					"Solicitud mas reciente obtenida."));
		}



		//Listado de solicitudes de cada Yonke 
		[HttpGet("MisSolicitudes")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado")]
		public async Task<ActionResult<ApiResponseGlobal<List<SolicitudYonke_List_DTO>>>> ObtenerMisSolicitudes(CancellationToken cancellationToken)
		{
			var yonkeGuidId = _currentUserService.YonkeGuidId;

			if (!yonkeGuidId.HasValue || yonkeGuidId.Value == Guid.Empty)
			{
				return Unauthorized(
					ApiResponseGlobal<string>.Fail(
						"No fue posible identificar el yonke autenticado."));
			}

			var solicitudes = await _solicitudYonkeService
				.ObtenerSolicitudesPorYonkeAsync(
				yonkeGuidId.Value, cancellationToken);

			return Ok(
				ApiResponseGlobal<List<SolicitudYonke_List_DTO>>.Ok(
					solicitudes,
					"Solicitudes obtenidas correctamente."));
		}




	}
}
