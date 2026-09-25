using AutoMapper;
using Core.DTO.Empresas;
using Core.Interfaces.Negocio.Coberturas;
using Core.ResponseGlobal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Asociados
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]
	public class YonkesCoberturasController : ControllerBase
	{
	
		private readonly IYunkeCoberturaService _yunkeCoberturaService;
		private readonly IMapper _mapper;

		
		public YonkesCoberturasController( IYunkeCoberturaService yunkeCoberturaService,
										  IMapper mapper)
		{			
			_yunkeCoberturaService = yunkeCoberturaService;
			_mapper = mapper;

		}




		/// <summary>
		/// Obtener cobertura de un yonke
		/// </summary>
		/// <param name="YonkeGuidId"></param>
		/// <returns></returns>
		[HttpGet("guid/{yonkeGuidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Soporte, Cliente")]
		public async Task<IActionResult> ObtenerCoberturasPorGuid(Guid yonkeGuidId)
		{
			try
			{
				var coberturas = await _yunkeCoberturaService
					.ObtenerPorYonkeGuidAsync(yonkeGuidId);

				return Ok(ApiResponseHelper.Success(
					coberturas,
					"Coberturas obtenidas correctamente."));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					ApiResponseHelper.Error(
						"Ocurrió un error al obtener las coberturas.",
						StatusCodes.Status500InternalServerError,
						ex.Message));
			}
		}



		/// <summary>
		/// M;odificar coberturas
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Soporte")]
		public async Task<IActionResult> ActualizarCoberturas([FromBody] YunkeCoberturaDTO dto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponseHelper.Error("Datos invalidos.", 400));
				}

				await _yunkeCoberturaService
					.ActualizarCoberturasAsync(dto.YonkeGuidId, dto.CiudadesIds);

				return Ok(ApiResponseHelper.Success(dto, "Cobertura actualizadas correctamente."));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					ApiResponseHelper.Error(
						"Ocurrió un error al actualizar las coberturas.",
						StatusCodes.Status500InternalServerError,
						ex.Message));
			}

		}




	}
}
