using AutoMapper;
using Core.DTO.Empresas;
using Core.Entitys;
using Core.Interfaces.Negocio.YunkeDispotivos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Asociados
{
	[Route("api/[controller]")]
	[ApiController]
	public class YonkesDispositivosController : ControllerBase
	{
		private readonly IYunkeDispositivoService _service;
		private readonly IMapper _mapper;

		public YonkesDispositivosController(IYunkeDispositivoService service,
									       IMapper mapper)
		{
			_service = service;
			_mapper = mapper;
		}


		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Soporte, Asociado")]
		public async Task<IActionResult> Registrar([FromBody] RegistrarDispositivoDto dto,	CancellationToken cancellationToken)
		{
			var dispositivo = _mapper.Map<YonkesDispositivos>(dto);

			await _service.AgregarAsync(dispositivo, cancellationToken);

			return Ok(new
			{
				success = true,
				message = "Dispositivo registrado correctamente."
			});
		}


	}
}
