using Core.DTO.Solicitudes.imagnees;
using Core.Exceptions;
using Core.Interfaces.Requests.imagenes;
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
	public class SolicitudesImagenesController : ControllerBase
	{
		private readonly ISolicitudesImagenesService _service;

		public SolicitudesImagenesController(ISolicitudesImagenesService service)
		{
			_service = service;
		}

		/// <summary>
		/// Agrega una imagenes a una solicitud.
		/// </summary>
		[HttpPost("{solicitudGuidId:guid}")]
		[Consumes("multipart/form-data")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> AgregarImagenes(Guid solicitudGuidId, List<IFormFile> imagenes, CancellationToken cancellationToken)
		{
			if (imagenes == null || !imagenes.Any())
			{
				return BadRequest(ApiResponseGlobal<string>.Fail("Debe seleccionar al menos una imagen."));
			}

			const int maxImagenes = 3;

			if (imagenes.Count > maxImagenes)
			{
				return BadRequest(ApiResponseGlobal<string>.Fail($"Solo se permiten hasta {maxImagenes} imágenes."));
			}

			try
			{
				var guids = await _service.AgregarImagenesAsync(solicitudGuidId, imagenes, cancellationToken);

				return Ok(ApiResponseGlobal<IList<Guid>>.Ok(guids, $"{guids.Count} imagen(es) agregada(s) correctamente."));
			}
			catch (BusinessException ex)
			{
				return BadRequest(ApiResponseGlobal<string>.Fail(ex.Message));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseGlobal<string>.Fail(ex.Message, 500));
			}
		}

		/// <summary>
		/// Obtiene todas las imágenes de una solicitud.
		/// </summary>
		[HttpGet("solicitud/{solicitudGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Asociado")]
		public async Task<IActionResult> GetImagenesSolicitud(Guid solicitudGuidId)
		{
			try
			{
				var data = await _service.GetImagenesBySolicitudAsync(solicitudGuidId);

				return Ok(ApiResponseGlobal<IList<SolicitudesImagenes_List_DTO>>.Ok(data));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseGlobal<IList<SolicitudesImagenes_List_DTO>>.Fail(ex.Message, 500));
			}
		}

		/// <summary>
		/// Obtiene una imagen.
		/// </summary>
		[HttpGet("{imagenGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Asociado")]
		public async Task<IActionResult> GetImagen(Guid imagenGuidId)
		{
			try
			{
				var data = await _service.GetImagenByGuidIdAsync(imagenGuidId);

				if (data == null)
				{
					return NotFound(ApiResponseGlobal<string>.Fail("Imagen no encontrada.",	404));
				}

				return Ok(ApiResponseGlobal<SolicitudesImagenes_DTO>.Ok(data));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseGlobal<SolicitudesImagenes_DTO>.Fail(ex.Message, 500));
			}
		}

		/// <summary>
		/// Elimina una imagen.
		/// </summary>
		[HttpDelete("{imagenGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> EliminarImagen(Guid imagenGuidId)
		{
			try
			{
				var eliminado = await _service.EliminarImagenAsync(	imagenGuidId);

				if (!eliminado)
				{
					return NotFound(ApiResponseGlobal<string>.Fail("Imagen no encontrada.",	404));
				}

				return Ok(ApiResponseGlobal<string>.Ok(string.Empty, "Imagen eliminada correctamente."));
			}
			catch (Exception ex)
			{
				return StatusCode(500,	ApiResponseGlobal<string>.Fail(ex.Message, 500));
			}
		}
	}
}
