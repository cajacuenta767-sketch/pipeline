using Core.Interfaces.BuildSecurity;
using Microsoft.AspNetCore.Http;

namespace Core.Services.Soporte.AccesoDetalles
{
	using System.Security.Claims;

	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Guid? UserId
		{
			get
			{
				var value = _httpContextAccessor.HttpContext?
					.User
					.FindFirstValue(ClaimTypes.NameIdentifier);

				return Guid.TryParse(value, out var guid)
					? guid
					: null;
			}
		}

		public string? IdentityUserId =>
			_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		public string? Email =>
			_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

		public string? Nombre =>
			_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

		public int? YunkeId
		{
			get
			{
				var value = _httpContextAccessor.HttpContext?.User.FindFirst("YunkeId")?.Value;

				return int.TryParse(value, out var id) ? id : null;
			}
		}

		public Guid? YonkeGuidId
		{
			get
			{
				var value = _httpContextAccessor.HttpContext?.User.FindFirst("YonkeGuidId")?.Value;

				return Guid.TryParse(value, out var guid) ? guid : null;
			}
		}
	}
}
