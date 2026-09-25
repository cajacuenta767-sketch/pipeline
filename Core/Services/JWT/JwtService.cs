using Core.DTO.JWT;
using Core.Entitys;
using Core.Interfaces.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services.JWT
{
	public class JwtService : IJwtService
	{
		private readonly JwtSettings _settings;

		public JwtService(IOptions<JwtSettings> settings)
		{
			_settings = settings.Value;
		}

		public string GenerarTokenCliente(Clientes cliente)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, cliente.GuidId.ToString()),
				new Claim(ClaimTypes.Name, cliente.Nombre ?? "Cliente refaNet"),
				new Claim(ClaimTypes.Role, "Cliente")
			};

			if (!string.IsNullOrWhiteSpace(cliente.Correo))
			{
				claims.Add(new Claim(JwtRegisteredClaimNames.Email,	cliente.Correo));
			}

			if (!string.IsNullOrWhiteSpace(cliente.Telefono))
			{
				claims.Add(new Claim(ClaimTypes.MobilePhone, cliente.Telefono));
			}



			return GenerarToken(claims);
		}

		public string GenerarTokenYunke(Yonkes yonke, IdentityUser user, IEnumerable<string> roles)
		{
			var claims = new List<Claim>
			{
			   // Claims estándar
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

				// Identity
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, yonke.Nombre),

				// Claims personalizados de RefaNet
				new Claim("YunkeId", yonke.Id.ToString()),
				new Claim("YonkeGuidId", yonke.GuidId.ToString()),
				new Claim("IdentityUserId", user.Id)
			};

			// Agregar todos los roles del usuario
			foreach (var rol in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, rol));
			}

			return GenerarToken(claims);
		}

		private string GenerarToken(IEnumerable<Claim> claims)
		{
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_settings.Key));

			var credentials = new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _settings.Issuer,
				audience: _settings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
