using Core.Entitys;
using Core.Interfaces.Login;
using Core.Interfaces.Login.ClienteOtp;
using Core.Interfaces.Login_Cliente;

namespace Core.Services.Login
{
	public class ClienteOtpService : IClienteOtpService
	{
		private readonly IUnitOfWorkCliente _unitOfWorkCliente;
		private readonly IClienteOtpRepository _otpRepository;
		private readonly ISmsService _smsService;

		private const int MaxIntentos = 5;
		private const int MinutosBloqueo = 15;

		public ClienteOtpService(
			IUnitOfWorkCliente unitOfWorkCliente,
			IClienteOtpRepository otpRepository,
			ISmsService smsService)
		{
			_unitOfWorkCliente = unitOfWorkCliente;
			_otpRepository = otpRepository;
			_smsService = smsService;
		}

		public async Task SolicitarOtpAsync(string telefono, string? ip, CancellationToken cancellationToken = default)
		{		

			telefono = NormalizarTelefono(telefono);

			// ==========================================
			// 1. VALIDAR BLOQUEO DEL TELÉFONO
			// ==========================================

			var bloqueo = await _otpRepository.ObtenerBloqueoTelefonoAsync(
				telefono,
				cancellationToken);

			if (bloqueo != null &&
				bloqueo.BloqueadoHasta.HasValue &&
				bloqueo.BloqueadoHasta.Value > DateTime.UtcNow)
			{
				var minutosRestantes =
					(int)Math.Ceiling(
						(bloqueo.BloqueadoHasta.Value - DateTime.UtcNow)
						.TotalMinutes);

				throw new InvalidOperationException(
					$"El acceso está bloqueado temporalmente. " +
					$"Intenta nuevamente en {minutosRestantes} minutos.");
			}

			// ==========================================
			// 1. BUSCAR CLIENTE
			// ==========================================

			var cliente =
				await _unitOfWorkCliente.LoginRepositorio
					.ObtenerPorTelefonoAsync(telefono);

			// ==========================================
			// 2. CREAR CLIENTE SI NO EXISTE
			// ==========================================

			if (cliente == null)
			{
				cliente = new Clientes
				{					
					GuidId = Guid.NewGuid(),
					Correo = null,
					AppleId = null,
					GoogleId = null,
					Telefono = telefono,
					TelefonoConfirmado = true,
					Activo = true,
					FechaRegistro = DateTime.UtcNow,
					Nombre = "Cliente refaNet"
				};

				await _unitOfWorkCliente.LoginRepositorio
					.AgregarAsync(cliente);

				await _unitOfWorkCliente.SaveChangesAsync();
			}

			// ==========================================
			// 3. DESACTIVAR OTP ANTERIORES
			// ==========================================

			await _otpRepository
				.DesactivarOtpsAnterioresAsync(
					telefono,
					cancellationToken);

			// ==========================================
			// 4. GENERAR OTP
			// ==========================================

			var codigo = GenerarCodigo();

			// ==========================================
			// 5. CREAR OTP
			// ==========================================

			var otp = new ClienteOtps
			{
				UserId = Guid.NewGuid().ToString(),
				ClienteGuidId = cliente.GuidId,
				PhoneNumber = telefono,
				CodigoHash = CalcularHash(codigo),
				FechaCreacion = DateTime.UtcNow,
				FechaExpiracion = DateTime.UtcNow.AddMinutes(5),
				Intentos = 0,
				Usado = false,
				Activo = true,
				Ip = ip
			};

			await _otpRepository.AgregarAsync(otp, cancellationToken);

			await _unitOfWorkCliente.SaveChangesAsync();

			// ==========================================
			// 6. ENVIAR SMS
			// ==========================================

			var mensaje =
				$"refaNet: tu código de acceso es {codigo}. " +
				"Expira en 5 minutos.";

			await _smsService.EnviarSmsAsync(
				telefono,
				mensaje,
				cancellationToken);
		}

		public async Task<Clientes> VerificarOtpAsync(string telefono, string codigo, CancellationToken cancellationToken = default)
		{
			telefono = NormalizarTelefono(telefono);

			// ==========================================
			// 1. OBTENER OTP
			// ==========================================

			var otp =
				await _otpRepository
					.ObtenerOtpActivoAsync(
						telefono,
						cancellationToken);

			if (otp == null)
				throw new InvalidOperationException(
					"El código no es válido.");

			// ==========================================
			// 2. VALIDAR EXPIRACIÓN
			// ==========================================

			if (otp.FechaExpiracion < DateTime.UtcNow)
			{
				otp.Activo = false;

				await _otpRepository.ActualizarAsync(
					otp,
					cancellationToken);

				await _unitOfWorkCliente.SaveChangesAsync();

				throw new InvalidOperationException(
					"El código ha expirado.");
			}

			// ==========================================
			// 3. VALIDAR INTENTOS
			// ==========================================

			if (otp.Intentos >= 5)
			{
				otp.Activo = false;

				await _otpRepository.ActualizarAsync(
					otp,
					cancellationToken);

				await _unitOfWorkCliente.SaveChangesAsync();

				throw new InvalidOperationException(
					"Has superado el número máximo de intentos.");
			}

			// ==========================================
			// 4. INCREMENTAR INTENTO
			// ==========================================

			otp.Intentos++;

			// ==========================================
			// 5. VALIDAR CÓDIGO
			// ==========================================

			var hash = CalcularHash(codigo);

			if (hash != otp.CodigoHash)
			{
				// ==========================================
				// BLOQUEAR DESPUÉS DE 5 INTENTOS FALLIDOS
				// ==========================================

				if (otp.Intentos >= 5)
				{
					otp.Activo = false;

					otp.BloqueadoHasta =
						DateTime.UtcNow.AddMinutes(15);
				}

				await _otpRepository.ActualizarAsync(
					otp,
					cancellationToken);

				await _unitOfWorkCliente.SaveChangesAsync();

				if (otp.Intentos >= 5)
				{
					throw new InvalidOperationException(
						"Has superado el número máximo de intentos. " +
						"El acceso ha sido bloqueado durante 15 minutos.");
				}

				throw new InvalidOperationException("El código no es válido.");
			}

			// ==========================================
			// 6. OTP CORRECTO
			// ==========================================

			otp.Usado = true;
			otp.Activo = false;
			otp.FechaVerificacion = DateTime.UtcNow;

			await _otpRepository.ActualizarAsync(
				otp,
				cancellationToken);

			// ==========================================
			// 7. OBTENER CLIENTE
			// ==========================================

			var cliente =
				await _unitOfWorkCliente.LoginRepositorio
					.ObtenerPorGuidAsync(
						otp.ClienteGuidId);

			if (cliente == null)
				throw new InvalidOperationException(
					"No se encontró el cliente.");

			// ==========================================
			// 8. CONFIRMAR TELÉFONO
			// ==========================================

			cliente.TelefonoConfirmado = true;

			await _unitOfWorkCliente.SaveChangesAsync();

			return cliente;
		}






		private static string GenerarCodigo()
		{
			return Random.Shared
				.Next(100000, 999999)
				.ToString();
		}

		private static string CalcularHash(string codigo)
		{
			using var sha256 =
				System.Security.Cryptography.SHA256.Create();

			var bytes =
				System.Text.Encoding.UTF8.GetBytes(codigo);

			var hash =
				sha256.ComputeHash(bytes);

			return Convert.ToHexString(hash);
		}

		private static string NormalizarTelefono(string telefono)
		{
			telefono = telefono
				.Trim()
				.Replace(" ", "")
				.Replace("-", "")
				.Replace("(", "")
				.Replace(")", "");

			if (!telefono.StartsWith("+"))
			{
				telefono = "+52" + telefono;
			}

			return telefono;
		}
	}
}
