using Core.DTO.Google_Login;
using Core.DTO.Login;
using Core.DTO.Login.otp;
using Core.Entitys;
using Core.Interfaces.JWT;
using Core.Interfaces.Login.AppleToken;
using Core.Interfaces.Login.ClienteOtp;
using Core.Interfaces.Login_Cliente;
using Core.Interfaces.Login_Cliente.GoogleApple;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Core.Services.Google_Login
{
	public class ClienteAuthService : IClienteAuthService
	{
		private readonly IUnitOfWorkCliente _unitOfWorkCliente;
		private readonly IJwtService _jwtService;
		private readonly IAppleTokenService _appleTokenService;
		private readonly IClienteOtpService _otpService;

		private readonly IConfiguration _configuration;



		public ClienteAuthService(
			IUnitOfWorkCliente unitOfWorkCliente,
			IJwtService jwtService,
			IAppleTokenService appleTokenService,
			IClienteOtpService clienteOtpService, 
			IConfiguration configuration)
		{
			_unitOfWorkCliente = unitOfWorkCliente;
			_jwtService = jwtService;
			_appleTokenService = appleTokenService;
			_otpService = clienteOtpService;
			_configuration = configuration;
		}


		// ============================================================
		// GOOGLE LOGIN
		// ============================================================

		public async Task<LoginClienteResponse> LoginGoogleAsync(GoogleLoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.IdToken))
			{
				throw new ArgumentException(
					"Token de Google inválido.");
			}

			// ========================================================
			// 1. VALIDAR TOKEN DE GOOGLE
			// ========================================================

			GoogleJsonWebSignature.Payload payload;

			

			try
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[]
					{
						_configuration["Google:ClientId"]
					}
				};

				payload = await GoogleJsonWebSignature.ValidateAsync(
					request.IdToken, settings);
			}
			catch
			{
				throw new UnauthorizedAccessException(
					"El token de Google no es válido.");
			}

			// ========================================================
			// 2. VALIDAR DATOS IMPORTANTES
			// ========================================================

			if (string.IsNullOrWhiteSpace(payload.Email))
			{
				throw new UnauthorizedAccessException(
					"Google no proporcionó un correo electrónico.");
			}

			if (!payload.EmailVerified)
			{
				throw new UnauthorizedAccessException(
					"El correo de Google no está verificado.");
			}

			// ========================================================
			// 3. BUSCAR CLIENTE POR CORREO
			// ========================================================

			var cliente =
				await _unitOfWorkCliente
					.LoginRepositorio
					.ObtenerPorCorreoAsync(payload.Email);

			// ========================================================
			// 4. CREAR CLIENTE SI NO EXISTE
			// ========================================================

			if (cliente == null)
			{
				cliente = new Clientes
				{
					GuidId = Guid.NewGuid(),
					Nombre = payload.Name,
					Correo = payload.Email,
					GoogleId = payload.Subject,
					FotoPerfil = payload.Picture,
					Activo = true,
					FechaRegistro = DateTime.UtcNow
				};

				await _unitOfWorkCliente
					.LoginRepositorio
					.AgregarAsync(cliente);

				await _unitOfWorkCliente
					.SaveChangesAsync();
			}
			else
			{
				// ====================================================
				// 5. ACTUALIZAR DATOS DE GOOGLE
				// ====================================================

				if (string.IsNullOrWhiteSpace(cliente.GoogleId))
				{
					cliente.GoogleId = payload.Subject;
				}

				if (!string.IsNullOrWhiteSpace(payload.Picture))
				{
					cliente.FotoPerfil = payload.Picture;
				}

				if (!string.IsNullOrWhiteSpace(payload.Name))
				{
					cliente.Nombre = payload.Name;
				}

				await _unitOfWorkCliente
					.SaveChangesAsync();
			}

			// ========================================================
			// 6. VALIDAR CLIENTE ACTIVO
			// ========================================================

			if (!cliente.Activo)
			{
				throw new UnauthorizedAccessException(
					"El cliente se encuentra inactivo.");
			}

			// ========================================================
			// 7. GENERAR JWT REFA NET
			// ========================================================

			var token =	_jwtService.GenerarTokenCliente(cliente);

			// ========================================================
			// 8. RESPUESTA
			// ========================================================

			return CrearLoginResponse(
				cliente,
				token);
		}
    


		// ============================================================
		// APPLE
		// ============================================================

		public async Task<LoginClienteResponse> LoginAppleAsync(AppleLoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.IdentityToken))
			{
				throw new ArgumentException(
					"IdentityToken de Apple inválido.");
			}


			// ========================================================
			// 1. VALIDAR TOKEN APPLE
			// ========================================================

			var applePayload =
				await _appleTokenService.ValidateAsync(
					request.IdentityToken);


			// ========================================================
			// 2. APPLE USER ID
			// ========================================================

			var appleId = applePayload.Subject;


			// ========================================================
			// 3. BUSCAR POR APPLE ID
			// ========================================================

			var cliente =
				await _unitOfWorkCliente
					.LoginRepositorio
					.ObtenerPorAppleIdAsync(appleId);


			// ========================================================
			// 4. SI NO EXISTE, BUSCAR POR CORREO
			// ========================================================

			if (cliente == null &&
				!string.IsNullOrWhiteSpace(
					applePayload.Email))
			{
				cliente =
					await _unitOfWorkCliente
						.LoginRepositorio
						.ObtenerPorCorreoAsync(
							applePayload.Email);
			}


			// ========================================================
			// 5. CREAR CLIENTE
			// ========================================================

			if (cliente == null)
			{
				var nombre =
					$"{request.Nombre} {request.Apellido}"
						.Trim();

				if (string.IsNullOrWhiteSpace(nombre))
				{
					nombre = "Cliente refaNet";
				}

				cliente = new Clientes
				{
					GuidId = Guid.NewGuid(),
					Nombre = nombre,
					Correo = applePayload.Email,
					AppleId = appleId,
					FotoPerfil = null,
					Activo = true,
					FechaRegistro = DateTime.UtcNow
				};

				await _unitOfWorkCliente
					.LoginRepositorio
					.AgregarAsync(cliente);

				await _unitOfWorkCliente
					.SaveChangesAsync();
			}
			else
			{
				// ====================================================
				// 6. VINCULAR APPLE A CLIENTE EXISTENTE
				// ====================================================

				if (string.IsNullOrWhiteSpace(cliente.AppleId))
				{
					cliente.AppleId = appleId;

					await _unitOfWorkCliente
						.SaveChangesAsync();
				}
			}


			// ========================================================
			// 7. GENERAR JWT
			// ========================================================

			var token =	_jwtService.GenerarTokenCliente(cliente);


			// ========================================================
			// 8. RESPUESTA
			// ========================================================

			return CrearLoginResponse(cliente, token);
		}


		// ============================================================
		// SOLICITAR OTP SMS
		// ============================================================

		public async Task SolicitarOtpAsync(SolicitarOtpRequest request, string? ip, CancellationToken cancellationToken = default)
		{
			if (request == null)
				throw new ArgumentNullException(
					nameof(request));

			if (string.IsNullOrWhiteSpace(
				request.Telefono))
			{
				throw new ArgumentException(
					"El número de teléfono es obligatorio.");
			}


			await _otpService.SolicitarOtpAsync(
				request.Telefono,
				ip,
				cancellationToken);
		}


		// ============================================================
		// VERIFICAR OTP SMS
		// ============================================================

		public async Task<LoginClienteResponse> VerificarOtpAsync(VerificarOtpRequest request, CancellationToken cancellationToken = default)
		{
			if (request == null)
				throw new ArgumentNullException(
					nameof(request));

			if (string.IsNullOrWhiteSpace(
				request.Telefono))
			{
				throw new ArgumentException(
					"El número de teléfono es obligatorio.");
			}

			if (string.IsNullOrWhiteSpace(
				request.Codigo))
			{
				throw new ArgumentException(
					"El código OTP es obligatorio.");
			}


			// ========================================================
			// 1. VERIFICAR OTP
			// ========================================================

			var cliente =
				await _otpService.VerificarOtpAsync(
					request.Telefono,
					request.Codigo,
					cancellationToken);


			// ========================================================
			// 2. GENERAR JWT REFA NET
			// ========================================================

			// IMPORTANTE:
			// GenerarTokenCliente devuelve string,
			// por eso NO lleva await.

			var token =
				_jwtService.GenerarTokenCliente(cliente);


			// ========================================================
			// 3. RESPUESTA
			// ========================================================

			return CrearLoginResponse(
				cliente,
				token);
		}


		// ============================================================
		// CREAR RESPUESTA DE LOGIN
		// ============================================================

		private static LoginClienteResponse CrearLoginResponse(Clientes cliente, string token)
		{
			return new LoginClienteResponse
			{
				Token = token,
				ClienteGuidId = cliente.GuidId,
				Nombre = cliente.Nombre,
				Correo = cliente.Correo,
				FotoPerfil = cliente.FotoPerfil
			};
		}


		// ============================================================
		// OBTENER PERFIL PUBLICO POR GUID
		// ============================================================

		public async Task<ClientePerfilDTO?> ObtenerPerfilPublicoAsync(Guid clienteGuidId)
		{
			if (clienteGuidId == Guid.Empty)
				return null;

			var cliente = await _unitOfWorkCliente
				.LoginRepositorio
				.ObtenerPorGuidAsync(clienteGuidId);

			if (cliente == null)
				return null;

			return new ClientePerfilDTO
			{
				GuidId = cliente.GuidId,
				Nombre = cliente.Nombre,
				FotoPerfil = cliente.FotoPerfil,
				Telefono = cliente.Telefono,
				Correo = cliente.Correo
			};
		}
	}
}

