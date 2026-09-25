using AutoMapper;
using Core.DTO.Empresas;
using Core.Entitys;
using Core.Interfaces.Auth.AccesoDetalle;
using Core.Interfaces.Azure;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Negocio.Empresa;
using Core.Interfaces.Utilerias.Citys;
using Core.Pagination;
using Core.ResponseGlobal;
using Infra.DataContext;
using Infra.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meta = Core.Pagination.Meta;

namespace ApiYonke.Controllers.Asociados
{
	//devuuelve lectura en formato json 
	[Produces("application/json")]

	//definicion de la ruta en url - endpoint
	[Route("api/[controller]")]

	//validacion del modelo en automatico
	[ApiController]


	public class YonkesController : ControllerBase
    {
		private readonly IYonkeservice _Yonkeservice;
		private readonly ICiudadService _ciudadService;
		private readonly IMapper _mapper;

		private readonly AplicationDBContext _context;

		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IAlmacenadorArchivos _almacenadorArchivos;

		private UserManager<IdentityUser> _userManger;

		//servicio de registro de usuarios
		private readonly IUserService _userService;

		//registro de acceso detalles de cada empresa
		private readonly IAccesoDetalleService _accesoDetalleService;

		//nombre de la carpeta donde se graban los datos en Azure
		private readonly string contenedor = "logos";

		public YonkesController(IYonkeservice Yonkeservice,
								  ICiudadService ciudadService,
								  IMapper mapper,
								  IWebHostEnvironment hostEnvironment,
								  IAlmacenadorArchivos almacenadorArchivos,
								  AplicationDBContext context,
								  IUserService userService,
								  IAccesoDetalleService accesoDetalleService,
								  UserManager<IdentityUser> userManager)
		{
			_Yonkeservice = Yonkeservice;
			_ciudadService = ciudadService;
			_mapper = mapper;
			_hostEnvironment = hostEnvironment;
			_almacenadorArchivos = almacenadorArchivos;
			_context = context;
			_userService = userService;
			_accesoDetalleService = accesoDetalleService;
			_userManger = userManager;
		}


		#region Consultar por ciudad y paginados
		/// <summary>
		/// Consultar todas los Yonkes registradas
		/// </summary>
		/// <param name="paginacionDTO"></param>
		/// <param name="ciudadId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("byPage")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente, Soporte")]
		public async Task<IActionResult> GetEmpresasByCiudad([FromQuery] PaginacionDTO paginacionDTO, int? ciudadId)
		{

			int total = 0;

			var datos = await _Yonkeservice.getYonkesByEntidad(ciudadId);

			if (paginacionDTO.Search != null)
			{
				//buscar por nombre de viajes
				var queryable = datos
					.Where(x => x.Nombre.Contains(paginacionDTO.Search) ||
						   x.Responsable.Contains(paginacionDTO.Search) ||
						   x.Telefono.Contains(paginacionDTO.Search))
					.AsQueryable();

				total = queryable.Count();

				await HttpContext.InsertarParametrosPaginados(queryable, paginacionDTO.CantidadRegistrosPorPagina);

				int total_pages = total / paginacionDTO.CantidadRegistrosPorPagina;
				if (total_pages <= 1)
				{
					total_pages = 1;
				}

				var incidentes = await queryable.Paginar(paginacionDTO).ToListAsync();

				if (incidentes.Count == 0)
				{
					var entlist1 = new ListarWithMetaPageDTO
					{
						data = { },

						meta = new Meta
						{
							page = paginacionDTO.Page,
							take = paginacionDTO.CantidadRegistrosPorPagina,
							itemCount = total,
							pageCount = total_pages
						}
					};
				}

				var entlist = new ListarWithMetaPageDTO
				{
					data = incidentes.ToArray(),

					meta = new Meta
					{
						page = paginacionDTO.Page,
						take = paginacionDTO.CantidadRegistrosPorPagina,
						itemCount = total,
						pageCount = total_pages
					}
				};

				return Ok(ApiResponseHelper.Success(entlist));

			}
			else
			{
				var queryable = datos.AsQueryable();

				total = queryable.Count();

				await HttpContext.InsertarParametrosPaginados(queryable, paginacionDTO.CantidadRegistrosPorPagina);

				int total_pages = total / paginacionDTO.CantidadRegistrosPorPagina;
				if (total_pages <= 1)
				{
					total_pages = 1;
				}

				var incidentes = await queryable.Paginar(paginacionDTO).ToListAsync();

				if (incidentes.Count == 0)
				{
					var entlist2 = new ListarWithMetaPageDTO
					{
						data = { },

						meta = new Meta
						{
							page = paginacionDTO.Page,
							take = paginacionDTO.CantidadRegistrosPorPagina,
							itemCount = total,
							pageCount = total_pages
						}
					};
				}

				var entlist = new ListarWithMetaPageDTO
				{
					data = incidentes.ToArray(),

					meta = new Meta
					{
						page = paginacionDTO.Page,
						take = paginacionDTO.CantidadRegistrosPorPagina,
						itemCount = total,
						pageCount = total_pages
					}
				};

				return Ok(ApiResponseHelper.Success(entlist));
			}



		}
		#endregion

		#region By Guid Id
		/// <summary>
		/// Buscar por Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{guidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Asociado, Cliente, Soporte")]
		public async Task<IActionResult> GetProductoById(Guid guidId)
		{
			var respuesta = await _Yonkeservice.getYunkeByGuidId(guidId);

			if (respuesta == null)
			{
				return NotFound(ApiResponseHelper.Error(
					"Empresa no encontrada.",
					StatusCodes.Status404NotFound));
			}

			var datos = _mapper.Map<YonkesListDTO>(respuesta);

			return Ok(ApiResponseHelper.Success(
				datos,
				"Empresa obtenida correctamente."));
		}
		#endregion

		#region Grabar yunke con su logotipo (Nueva empresa)
		/// <summary>
		/// Grabar nueva yunke
		/// </summary>
		/// <param name="empresaDTO"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte")]
		public async Task<IActionResult> GrabarYonke([FromForm] YonkesaveDTO model, CancellationToken cancellationToken)
		{
			var resultado = await _Yonkeservice.NuevoYunkeAsync(model, cancellationToken);

			return Ok(ApiResponseHelper.Success("El yonke se creó correctamente."));
		}
		#endregion



		#region Update Informacion
		/// <summary>
		/// Actualizar informacion 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="empresaUpdateInfoDTO"></param>
		/// <returns></returns>		
		[HttpPut("updateInfo/byGuidId/{guidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte, Asociado")]
		public async Task<IActionResult> UpdateYunke(Guid guidId, [FromBody] YunkeUpdateInfoDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var empresa = _mapper.Map<Yonkes>(dto);

			empresa.GuidId = guidId;

			var resultado = await _Yonkeservice.UpdateYunke(empresa);

			if (!resultado)
				return BadRequest(new { mensaje = "No fue posible actualizar el yonke." });

			return Ok(new { mensaje = "Yonke actualizado correctamente." });
		}
		#endregion

		#region Update Logotipo
		/// <summary>
		/// Update logotipo
		/// </summary>
		/// <param name="id"></param>
		/// <param name="empresaUpdateLogo"></param>
		/// <returns></returns>
		[HttpPut("ActualizarLogo")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte, Asociado")]
		public async Task<IActionResult> ActualizarLogo([FromForm] YunkeUpdateLogoDTO model)
		{
			if (model.LogoUrl == null || model.LogoUrl.Length == 0)
			{
				return BadRequest(new
				{
					mensaje = "Debe seleccionar una imagen."
				});
			}

			var resultado = await _Yonkeservice.ActualizarLogoAsync(model.GuidId, model.LogoUrl);

			if (!resultado)
			{
				return NotFound(new
				{
					mensaje = "Yunke no encontrado."
				});
			}

			return Ok(new
			{
				mensaje = "Logotipo actualizado correctamente."
			});
		}
		#endregion

		#region Baja empresa
		/// <summary>
		/// Baja de asociado
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("baja/byGuidId/{guidId:guid}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte")]
		public async Task<IActionResult> BajaEmpresa(Guid guidId)
		{
			#region Validar existencia del Yunke

			var yunkeDB = await _context.Yonkes
				.FirstOrDefaultAsync(x => x.GuidId == guidId);

			if (yunkeDB == null)
			{
				return NotFound(ApiResponseHelper.Error("Yunke no encontrado.", 404));
			}

			if (!yunkeDB.Estatus)
			{
				return BadRequest(ApiResponseHelper.Error("El Yunke ya está dado de baja.", 400));
			}

			#endregion


			#region Actualizar estatus

			var resultado = await _Yonkeservice.BajaYunke(yunkeDB.GuidId);

			if (!resultado)
			{
				return BadRequest(ApiResponseHelper.Error(
					"No fue posible actualizar el estatus del Yunke.",
					400));
			}

			#endregion


			return Ok(ApiResponseHelper.Success(new
			{
				mensaje = "Estatus actualizado correctamente.",
				guidId = yunkeDB.GuidId,
				estatus = false
			}));
		}
		#endregion


	}
}
