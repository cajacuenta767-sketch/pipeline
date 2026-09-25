using Core.Interfaces.Requests.ciudades;
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

	public class SolicitudCiudadesController : ControllerBase
	{
		private readonly ISolicitudCiudadesService _service;

		public SolicitudCiudadesController(ISolicitudCiudadesService service)
		{
			_service = service;
		}

		/// <summary>
		/// Agregar una ciudad a una solicitud
		/// </summary>
		[HttpPost("{solicitudGuidId}/ciudad/{ciudadId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> Agregar(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken)
		{
			await _service.AgregarAsync(solicitudGuidId, ciudadId, cancellationToken);

			return Ok(ApiResponseGlobal<object>.Ok(
			new
			{
				SolicitudGuidId = solicitudGuidId
			},
			"Ciudad agregada correctamente."));
		}


		/// <summary>
		/// Agregar varias ciudades a una solicitud
		/// </summary>
		[HttpPost("{solicitudGuidId}/ciudades")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> AgregarRango(Guid solicitudGuidId,	[FromBody] IList<int> ciudadesIds, CancellationToken cancellationToken)
		{
			await _service.AgregarRangoAsync(solicitudGuidId, ciudadesIds, cancellationToken);

			return Ok(ApiResponseGlobal<object>.Ok(
			new
			{
				SolicitudGuidId = solicitudGuidId
			},
			"Ciudades agregadas correctamente."));
		}


		/// <summary>
		/// Obtener ciudades asociadas a una solicitud
		/// </summary>
		[HttpGet("{solicitudGuidId}/ciudades")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Asociado")]
		public async Task<IActionResult> ObtenerPorSolicitud(Guid solicitudGuidId)
		{
			if (solicitudGuidId == Guid.Empty)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la solicitud no es válido."));
			}

			var result = await _service.ObtenerCiudadesPorSolicitudAsync(solicitudGuidId);

			if (result.SolicitudHeader.CiudadesSaveBySolicitud.Count == 0)
			{
				return BadRequest(ApiResponseHelper.Error(
					"No hay ciudades agregadas."));
			}

			return Ok(ApiResponseHelper.Success(result));
		}


		/// <summary>
		/// Validar si una ciudad existe en la solicitud
		/// </summary>
		[HttpGet("existe")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> Existe(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken)
		{
			if (solicitudGuidId == Guid.Empty)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la solicitud no es válido."));
			}

			if (ciudadId <= 0)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la ciudad no es válido."));
			}

			var existe = await _service.ExisteAsync(
				solicitudGuidId,
				ciudadId,
				cancellationToken);

			return Ok(ApiResponseHelper.Success(
				new
				{
					Existe = existe
				},
				"Consulta realizada correctamente."));
		}


		/// <summary>
		/// Reemplazar ciudades de una solicitud
		/// </summary>
		[HttpPut("{solicitudGuidId}/ciudades")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> Actualizar(Guid solicitudGuidId, [FromBody] IList<int> ciudadesIds, CancellationToken cancellationToken)
		{
			await _service.ActualizarAsync(solicitudGuidId,	ciudadesIds, cancellationToken);

			return Ok(ApiResponseHelper.Success(
			new
			{
				SolicitudGuidId = solicitudGuidId
			},
				"Ciudades actualizadas correctamente."
			));

		}


		/// <summary>
		/// Eliminar una ciudad de una solicitud
		/// </summary>
		[HttpDelete("{solicitudGuidId}/ciudad/{ciudadId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> Eliminar(Guid solicitudGuidId,	int ciudadId, CancellationToken cancellationToken)
		{
			if (solicitudGuidId == Guid.Empty)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la solicitud no es válido."));
			}

			if (ciudadId <= 0)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la ciudad no es válido."));
			}

			var eliminado = await _service.EliminarAsync(
				solicitudGuidId,
				ciudadId,
				cancellationToken);

			if (!eliminado)
			{
				return NotFound(ApiResponseHelper.Error(
					"No existe la ciudad asociada a la solicitud."));
			}

			return Ok(ApiResponseHelper.Success(
				true,
				"Ciudad eliminada correctamente."));
		}


		/// <summary>
		/// Eliminar todas las ciudades de una solicitud
		/// </summary>
		[HttpDelete("{solicitudGuidId}/ciudades")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> EliminarTodas(Guid solicitudGuidId, CancellationToken cancellationToken)
		{
			if (solicitudGuidId == Guid.Empty)
			{
				return BadRequest(ApiResponseHelper.Error(
					"El identificador de la solicitud no es válido."));
			}

			var eliminado = await _service.EliminarTodasAsync(
				solicitudGuidId,
				cancellationToken);

			if (!eliminado)
			{
				return NotFound(ApiResponseHelper.Error(
					"No existen ciudades asociadas a la solicitud."));
			}

			return Ok(ApiResponseHelper.Success(
				true,
				"Ciudades eliminadas correctamente."));
		}



	}
}
