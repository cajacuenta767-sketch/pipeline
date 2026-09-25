using Core.Interfaces.Requests.Solicitud;
using Core.Pagination;
using Core.ResponseGlobal;
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

	public class DashboardSuscriptoresController : ControllerBase
	{
		private readonly ISolicitudService _solicitudService;

		public DashboardSuscriptoresController(ISolicitudService solicitudService)
		{
			_solicitudService = solicitudService;
		}


		[HttpGet("resumen")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> ObtenerResumen()
		{
			var solicitudes = await _solicitudService.ContarSolicitudesUsuarioAsync();
			var cotizaciones = await _solicitudService.ContarCotizacionesUsuarioAsync();

			return Ok(new
			{
				totalSolicitudes = solicitudes,
				totalCotizaciones = cotizaciones
			});
		}


		[HttpGet("mis-solicitudes")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> VerSolicitudesByUserDashboard([FromQuery] PaginacionDTO paginacionDTO)
		{
			if (paginacionDTO.CantidadRegistrosPorPagina <= 0)
				paginacionDTO.CantidadRegistrosPorPagina = 10;

			if (paginacionDTO.Page <= 0)
				paginacionDTO.Page = 1;

			var queryable = _solicitudService
				.VerSolicitudesByUserDashboard()
				.AsNoTracking();

			// 🔍 Filtro de búsqueda
			if (!string.IsNullOrWhiteSpace(paginacionDTO.Search))
			{
				var search = paginacionDTO.Search.Trim();

				queryable = queryable.Where(x =>
					EF.Functions.Like(x.NumeroParte, $"%{search}%") ||
					EF.Functions.Like(x.Folio, $"%{search}%") ||
					EF.Functions.Like(x.PiezaBuscada, $"%{search}%") ||
					EF.Functions.Like(x.Descripcion, $"%{search}%")
				);
			}

			// 🔑 Ordenamiento
			queryable = queryable
				.OrderByDescending(x => x.FechaCreacion)
				.ThenByDescending(x => x.GuidId);

			// 📊 Total de registros
			var total = await queryable.CountAsync();

			var totalPages = (int)Math.Ceiling(
				(double)total / paginacionDTO.CantidadRegistrosPorPagina);

			// 📄 Registros de la página actual
			var registros = await queryable
				.Skip((paginacionDTO.Page - 1) *
					  paginacionDTO.CantidadRegistrosPorPagina)
				.Take(paginacionDTO.CantidadRegistrosPorPagina)
				.ToListAsync();

			// 📦 Resultado
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

			return Ok(
				ApiResponseHelper.Success(
					resultado,
					"Busqueda exitosa."));
		}


		[HttpGet("mis-cotizaciones")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> GetMisCotizaciones()
		{
			var cotizaciones = await _solicitudService
				.GetCotizacionesByUserId()
				.OrderByDescending(x => x.FechaCreacionCotizacion)
				.ToListAsync();

			return Ok(
				ApiResponseHelper.Success(
					cotizaciones,
					"Cotizaciones obtenidas correctamente."));
		}



		[HttpGet("mi-solicitud-reciente")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Cliente")]
		public async Task<IActionResult> ObtenerMiSolicitudMasReciente()
		{
			var solicitud = await _solicitudService
				.ObtenerSolicitudMasRecienteAsync();

			if (solicitud == null)
			{
				return NotFound(
					ApiResponseHelper.Error("No se encontraron solicitudes."));
			}

			return Ok(
				ApiResponseHelper.Success(
					solicitud,
					"Solicitud más reciente obtenida correctamente."));
		}




	}
}
