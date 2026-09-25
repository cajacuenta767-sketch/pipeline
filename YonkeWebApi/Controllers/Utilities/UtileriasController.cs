using AutoMapper;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Utilerias.brand;
using Core.Interfaces.Utilerias.Citys;
using Core.Interfaces.Utilerias.Models;
using Core.Interfaces.Utilerias.States;
using Core.ResponseGlobal;
using Humanizer;
using Infra.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;

namespace ApiYonke.Controllers.Utilities
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]


	public class UtileriasController : ControllerBase
	{
		public readonly AplicationDBContext _context;
		public readonly IMapper _mapper;
		public readonly ICiudadService _ciudadService;
		public readonly IStateService _stateService;
		public readonly IMarcaService _marcaService;
		public readonly IModeloService _modeloService;

		public UtileriasController(AplicationDBContext context,
								   IMapper mapper,
								   ICiudadService ciudadService,
								   IStateService stateService,
								   IMarcaService marcaService,
								   IModeloService modeloService)
		{
			_ciudadService = ciudadService;
			_stateService = stateService;
			_mapper = mapper;
			_context = context;	
			_marcaService = marcaService;
			_modeloService = modeloService;
		}



		#region Entidades
		/// <summary>
		/// Obtiene todos los estados
		/// </summary>
		[HttpGet("entidades")]
		[AllowAnonymous]
		public async Task<IActionResult> GetStates()
		{
			try
			{
				var states = await _stateService.getStates();

				return Ok(ApiResponseHelper.Success(states));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
			
		}

		/// <summary>
		/// Obtiene un estado por Id
		/// </summary>
		[HttpGet("entidad/{id:int}")]
		public async Task<IActionResult> GetStateById(int id)
		{
			try
			{
				var state = await _stateService.getStateById(id);

				if (state == null)
					return NotFound(ApiResponseHelper.Error("No se encontro la entidad.", 404));

				return Ok(ApiResponseHelper.Success(state));
			}
			catch (Exception ex)
			{

				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
			
		}
		#endregion


		#region Ciudades
		/// <summary>
		/// Ciudades por Entidad
		/// </summary>
		/// <param name="entidadId"></param>
		/// <returns></returns>
		[HttpGet("entidad/{entidadId:int}/ciudades")]
		[AllowAnonymous]
		public async Task<IActionResult> GetCiudades(int entidadId)
		{
			try
			{
				var ciudades = await _ciudadService.getCiudades(entidadId);

				return Ok(ApiResponseHelper.Success(ciudades));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
			
		}



		/// <summary>
		/// Ciudad by Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("ciudad/{id:int}")]
		public async Task<IActionResult> GetCiudadById(int id)
		{
			try
			{
				var ciudad = await _ciudadService.getCiudadById(id);

				if (ciudad == null)
					return NotFound(ApiResponseHelper.Error("No se encontro la ciudad.", 404));

				return Ok(ApiResponseHelper.Success(ciudad));
			}
			catch (Exception ex)
			{

				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
			
		}
		#endregion


		#region Marcas

		/// <summary>
		/// Obtiene todas las marcas
		/// </summary>
		[HttpGet("marcas")]
		[AllowAnonymous]
		public async Task<IActionResult> GetMarcas()
		{
			try
			{
				var marcas = await _marcaService.getMarcasListByEmpresa();

				return Ok(ApiResponseHelper.Success(marcas));
			}
			catch (Exception ex)
			{
				return StatusCode(500,	ApiResponseHelper.Error("Ocurrió un error inesperado.",	500, new { ex.Message }));
			}
		}

		/// <summary>
		/// Obtiene una marca por Id
		/// </summary>
		[HttpGet("marca/{id:int}")]
		public async Task<IActionResult> GetMarcaById(int id)
		{
			try
			{
				var marca = await _marcaService.getMarcaById(id);

				if (marca == null)
					return NotFound(ApiResponseHelper.Error("No se encontró la marca.",	404));

				return Ok(ApiResponseHelper.Success(marca));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error(
						"Ocurrió un error inesperado.",
						500,
						new { ex.Message }));
			}
		}

		/// <summary>
		/// Crear nueva marca
		/// </summary>
		[HttpPost("marca")]
		public async Task<IActionResult> CrearMarca([FromBody] Marcas marca)
		{
			try
			{
				await _marcaService.NuevoMarca(marca);

				return Ok(
					ApiResponseHelper.Success("Marca creada correctamente"));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error("Ocurrió un error inesperado.",	500, new { ex.Message }));
			}
		}

		/// <summary>
		/// Actualizar marca
		/// </summary>
		[HttpPut("marca/{id:int}")]
		public async Task<IActionResult> ActualizarMarca(
			int id,
			[FromBody] Marcas marca)
		{
			try
			{
				if (id != marca.Id)
				{
					return BadRequest(
						ApiResponseHelper.Error("El Id de la ruta no coincide con el Id enviado.", 400));
				}

				var existe = await _marcaService.ExisteMarca(id);

				if (!existe)
				{
					return NotFound(
						ApiResponseHelper.Error("No se encontró la marca.",	404));
				}

				await _marcaService.UpdateMarca(marca);

				return Ok(
					ApiResponseHelper.Success("Marca actualizada correctamente"));
			}
			catch (BusinessException ex)
			{
				return StatusCode(ex.StatusCode, ApiResponseHelper.Error(ex.Message, ex.StatusCode));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error("Ocurrió un error inesperado.",	500, new { ex.Message }));
			}
		}

		#endregion


		#region Modelos
		/// <summary>
		/// Obtener los modelos por marca
		/// </summary>
		/// <param name="marcaId"></param>
		/// <returns></returns>
		[HttpGet("modelos")]
		[AllowAnonymous]
		public async Task<IActionResult> GetModelos(int marcaId)
		{
			try
			{
				var modelos = await _modeloService.getModelosListByEmpresa(marcaId);

				return Ok(ApiResponseHelper.Success(modelos));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
		}

		/// <summary>
		/// Obtiene una modelo por su Id
		/// </summary>
		[HttpGet("modelo/{id:int}")]
		public async Task<IActionResult> GetModeloById(int id)
		{
			try
			{
				var modelo = await _modeloService.getModeloById(id);

				if (modelo == null)
					return NotFound(ApiResponseHelper.Error("No se encontró el modelo.", 404));

				return Ok(ApiResponseHelper.Success(modelo));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error(
						"Ocurrió un error inesperado.",
						500,
						new { ex.Message }));
			}
		}

		/// <summary>
		/// Crear nuevo modelo
		/// </summary>
		[HttpPost("modelo")]
		public async Task<IActionResult> CrearModelo([FromBody] Modelos modelo)
		{
			try
			{
				await _modeloService.NuevoModelo(modelo);

				return Ok(
					ApiResponseHelper.Success("Modelo creada correctamente"));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
		}

		/// <summary>
		/// Actualizar modelo
		/// </summary>
		[HttpPut("modelo/{id:int}")]
		public async Task<IActionResult> ActualizarModelo(int id, [FromBody] Modelos modelo)
		{
			try
			{
				if (id != modelo.Id)
				{
					return BadRequest(ApiResponseHelper.Error("El Id de la ruta no coincide con el Id enviado.", 400));
				}

				var existe = await _modeloService.ExisteModelo(id);

				if (!existe)
				{
					return NotFound(ApiResponseHelper.Error("No se encontró la marca.", 404));
				}

				await _modeloService.UpdateModelo(modelo);

				return Ok(ApiResponseHelper.Success("Modelo actualizada correctamente"));
			}
			catch (BusinessException ex)
			{
				return StatusCode(ex.StatusCode, ApiResponseHelper.Error(ex.Message, ex.StatusCode));
			}
			catch (Exception ex)
			{
				return StatusCode(500,
					ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
		}
		#endregion
	}
}
