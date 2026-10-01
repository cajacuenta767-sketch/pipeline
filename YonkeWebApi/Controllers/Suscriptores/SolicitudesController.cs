using AutoMapper;
using Core.DTO.Solicitudes.Requests;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Requests.Estatus;
using Core.Interfaces.Requests.Solicitud;
using Core.Pagination;
using Core.ResponseGlobal;
using Infra.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiYonke.Controllers.Suscriptores
{

	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]


	public class SolicitudesController : ControllerBase
	{
		private readonly ISolicitudService _solicitudService;
		private readonly ISolicitudEstatusService _solicitudEstatusService;
		private readonly IMapper _mapper;

		private readonly AplicationDBContext _context;


		public SolicitudesController(ISolicitudService solicitudService,
									 ISolicitudEstatusService solicitudEstatusService,
								     IMapper mapper,
									 AplicationDBContext context)
		{
			_solicitudService = solicitudService;
			_solicitudEstatusService = solicitudEstatusService;
			_mapper = mapper;

			_context = context;
		}


		#region Consultar Solicitudes all paged
		/// <summary>
		/// Get Solicitudes paginados
		/// </summary>
		/// <param name="paginacionDTO"></param>
		/// <param name="desde"></param>
		/// <param name="hasta"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("AllPaged")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Administrador")]
		public async Task<IActionResult> getSolicitudesByUser([FromQuery] PaginacionDTO paginacionDTO,
															  DateTime desde, 
															  DateTime hasta, 
															  Guid userId)
		{
			if (paginacionDTO.CantidadRegistrosPorPagina <= 0)
				paginacionDTO.CantidadRegistrosPorPagina = 10;					

			var queryable = _solicitudService
				.GetSolicitudesByUserId(desde, hasta, userId)
				.AsNoTracking();

			// 🔍 Filtro optimizado
			if (!string.IsNullOrEmpty(paginacionDTO.Search))
			{
				var search = paginacionDTO.Search;

				queryable = queryable.Where(x =>
					EF.Functions.Like(x.NumeroParte, $"%{search}%") ||
					EF.Functions.Like(x.Folio, $"%{search}%") ||
					EF.Functions.Like(x.PiezaBuscada, $"%{search}%")
				);
			}

			// 🔑 ORDENAMIENTO obligatorio
			queryable = queryable
			.OrderByDescending(x => x.FechaCreacion)
			.ThenByDescending(x => x.GuidId);

			// ✅ Async real con EF
			int total = await queryable.CountAsync();

			int totalPages = (int)Math.Ceiling((double)total / paginacionDTO.CantidadRegistrosPorPagina);

			var registros = await queryable
				.Skip((paginacionDTO.Page - 1) * paginacionDTO.CantidadRegistrosPorPagina)
				.Take(paginacionDTO.CantidadRegistrosPorPagina)
				.ToListAsync();

			var resultado = new ListarWithMetaPageDTO
			{
				data = registros.ToArray(),
				meta = new Core.Pagination.Meta
				{
					page = paginacionDTO.Page,
					take = paginacionDTO.CantidadRegistrosPorPagina,
					itemCount = total,
					pageCount = totalPages
				}
			};

			//return Ok(resultado);
			return Ok(ApiResponseHelper.Success(resultado, "Busqueda exitosa."));
		}
		#endregion


		#region Solicitud by GuidId
		/// <summary>
		/// Solicitud by Id
		/// </summary>
		/// <param name="guidId"></param>
		/// <returns></returns>
		[HttpGet("{guidId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente, Asociado, Administrador, Soporte")]
		public async Task<ActionResult<ApiResponseGlobal<Solicitud_Busqueda_DTO>>> GetByGuidId(Guid guidId)
		{
			// Validación inicial
			if (guidId == Guid.Empty)
			{
				return BadRequest(ApiResponseHelper.Error("El guidId es requerido.", 400));
			}

			try
			{
				var data = await _solicitudService.GetSolicitudByGuidId(guidId);

				if (data == null)
					return NotFound(ApiResponseHelper.Error("No se encontró la solicitud", 404));

				return Ok(ApiResponseHelper.Success(data));
			}
			catch (Exception ex)
			{
				// Loguear el error aquí si es necesario
				return StatusCode(500, ApiResponseHelper.Error("Ocurrió un error inesperado.", 500, new { ex.Message }));
			}
		}


		#endregion


		#region Add New Solicitud
		/// <summary>
		/// Nueva Solicitud sin imagenes -- Proceso Add Imagenes
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>		
		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> NuevaSolicitud([FromBody] Solicitudes_Create_DTO dto, CancellationToken cancellationToken)
		{
			if (dto == null)
				return BadRequest(
					ApiResponseGlobal<string>.Fail("Datos inválidos"));

			try
			{
				var solicitud = await _solicitudService
					.NuevaSolicitud(dto, cancellationToken);

				return Ok(
					ApiResponseGlobal<string>.Ok(
						solicitud.GuidId.ToString(),
						"Solicitud creada correctamente"));
			}
			catch (BusinessException ex)
			{
				return BadRequest(
					ApiResponseGlobal<string>.Fail(ex.Message, 400));
			}
			catch (Exception ex)
			{
				return StatusCode(
					500,
					ApiResponseGlobal<string>.Fail(
						$"Error interno: {ex.Message}",
						500));
			}
		}

		#endregion



		#region Dar de baja una solicitud
		/// <summary>
		/// Cancelar Solicitud
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpDelete("{guidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> ActualizarEstatusSolicitud([FromBody] SolicitudUpdateStatusDTO dto)
		{
			if (IsNullOrEmpty(dto.GuidId))
				return BadRequest(ApiResponseHelper.Error("GuidId es requerido", 400));

			if (dto == null)
				return BadRequest(ApiResponseHelper.Error("Request inválido", 400));

			try
			{
				var result = await _solicitudService.UpdateStatusAsync(dto);

				return Ok(ApiResponseHelper.Success(result, "Solicitud cancelada correctamente."));
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ApiResponseHelper.Error(ex.Message, 404));
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ApiResponseHelper.Error(ex.Message, 400));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponseHelper.Error(ex.Message, 500));
			}
		}




		#endregion




		#region Consultar los estatus para la solicitud
		/// <summary>
		/// Consultar Estatus
		/// </summary>
		/// <returns></returns>
		//[HttpGet("estatus")]
		//public async Task<IActionResult> GetSolicitudEstatus()
		//{
		//	try
		//	{
		//		var estatus = await _solicitudEstatusService.GetSolicitudEstatusAsync();

		//		return Ok(ApiResponseGlobal<IList<Solicitud_Estatus_List_DTO>>
		//			.Ok(estatus, "Consulta realizada correctamente"));
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500,
		//			ApiResponseGlobal<IList<Solicitud_Estatus_List_DTO>>
		//				.Fail($"Error interno: {ex.Message}", 500));
		//	}
		//}
		#endregion


		private bool IsNullOrEmpty(Guid guidId)
		{
			return guidId == Guid.Empty;
		}
		
	}
}
