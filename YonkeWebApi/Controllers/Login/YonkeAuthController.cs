using Core.DTO.Login;
using Core.Interfaces.Login.Yonke;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiYonke.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class YonkeAuthController : ControllerBase
	{
		private readonly IYonkeAuthService _service;

		public YonkeAuthController(IYonkeAuthService service)
		{
			_service = service;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginYonkeRequest request)
		{
			return Ok(await _service.LoginAsync(request));
		}
	}
}
