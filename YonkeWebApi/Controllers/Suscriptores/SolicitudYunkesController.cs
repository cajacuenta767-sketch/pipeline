using Core.DTO.SolocitudCotizaciones;
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

		public SolicitudYonkesController(ISolicitudYonkeService solicitudYonkeService)
		{
			_solicitudYonkeService = solicitudYonkeService;
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



		


	}
}
