using Core.DTO.Login;
using Core.Interfaces.Login.AppleToken;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Services.Login
{	public class AppleTokenService : IAppleTokenService
	{
		private readonly IConfiguration _configuration;
		private const string AppleIssuer = "https://appleid.apple.com";
		private const string AppleKeysUrl =	"https://appleid.apple.com/auth/keys";

		public AppleTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<AppleTokenPayload> ValidateAsync(string identityToken)
		{
			if (string.IsNullOrWhiteSpace(identityToken))
				throw new Exception("IdentityToken de Apple inválido.");

			var clientId = _configuration["Apple:ClientId"];

			if (string.IsNullOrWhiteSpace(clientId))
				throw new Exception(
					"No está configurado Apple:ClientId.");

			var configurationManager =
				new ConfigurationManager<OpenIdConnectConfiguration>(
					AppleKeysUrl,
					new OpenIdConnectConfigurationRetriever(),
					new HttpDocumentRetriever
					{
						RequireHttps = true
					});

			var appleConfiguration = await configurationManager.GetConfigurationAsync();

			var validationParameters =
				new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = AppleIssuer,
					ValidateAudience = true,
					ValidAudience = clientId,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKeys = appleConfiguration.SigningKeys,
					ClockSkew = TimeSpan.FromMinutes(2)
				};

			var handler = new JwtSecurityTokenHandler();

			var principal = handler.ValidateToken(
				identityToken,
				validationParameters,
				out var validatedToken);

			var jwt = validatedToken as JwtSecurityToken;

			if (jwt == null)
				throw new SecurityTokenException(
					"El token de Apple no es válido.");

			var subject =
				principal.FindFirst("sub")?.Value;

			var email =
				principal.FindFirst("email")?.Value;

			var emailVerifiedClaim =
				principal.FindFirst("email_verified")?.Value;

			if (string.IsNullOrWhiteSpace(subject))
				throw new SecurityTokenException(
					"Apple no proporcionó el identificador del usuario.");

			bool emailVerified =
				string.Equals(
					emailVerifiedClaim,
					"true",
					StringComparison.OrdinalIgnoreCase);

			return new AppleTokenPayload
			{
				Subject = subject,
				Email = email,
				EmailVerified = emailVerified
			};
		}
	}
}
