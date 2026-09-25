using Core.DTO.JWT;
using Core.DTO.Login.AuthSoporte;
using Core.Exceptions;
using Core.Interfaces.Login.AuthSoporte;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services.Login
{
	public class AuthSoporte : IUsuarioService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		public AuthSoporte(UserManager<IdentityUser> userManager,
						   RoleManager<IdentityRole> roleManager,
						   IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
		}

		public async Task<string> CrearUsuarioSoporteAsync(CrearUsuarioSoporteDTO request, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// Validaciones
			// ==========================================

			if (string.IsNullOrWhiteSpace(request.Nombre))
				throw new BusinessException(
					"El nombre es obligatorio.");

			if (string.IsNullOrWhiteSpace(request.Email))
				throw new BusinessException(
					"El correo electrónico es obligatorio.");

			if (string.IsNullOrWhiteSpace(request.Password))
				throw new BusinessException(
					"La contraseña es obligatoria.");

			if (string.IsNullOrWhiteSpace(request.Role))
				throw new BusinessException(
					"El rol es obligatorio.");

			// ==========================================
			// Normalizar datos
			// ==========================================

			var email = request.Email.Trim();
			var nombreRol = request.Role.Trim();

			// ==========================================
			// Validar si ya existe
			// ==========================================

			var usuarioExistente =
				await _userManager.FindByEmailAsync(email);

			if (usuarioExistente != null)
			{
				throw new BusinessException(
					"Ya existe un usuario registrado con ese correo electrónico.");
			}

			// ==========================================
			// Validar que exista el rol
			// ==========================================

			var existeRol =
				await _roleManager.RoleExistsAsync(nombreRol);

			if (!existeRol)
			{
				throw new BusinessException(
					$"El rol '{nombreRol}' no existe.");
			}

			// ==========================================
			// Crear usuario
			// ==========================================

			var usuario = new IdentityUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};

			var resultado = await _userManager.CreateAsync(
				usuario,
				request.Password);

			if (!resultado.Succeeded)
			{
				var errores = string.Join(
					", ",
					resultado.Errors.Select(x => x.Description));

				throw new BusinessException(
					$"No fue posible crear el usuario: {errores}");
			}

			// ==========================================
			// Asignar rol enviado
			// ==========================================

			var resultadoRol =
				await _userManager.AddToRoleAsync(
					usuario,
					nombreRol);

			if (!resultadoRol.Succeeded)
			{
				// Si falla la asignación del rol,
				// eliminamos el usuario recién creado.

				await _userManager.DeleteAsync(usuario);

				var erroresRol = string.Join(
					", ",
					resultadoRol.Errors.Select(x => x.Description));

				throw new BusinessException(
					$"No fue posible asignar el rol '{nombreRol}': {erroresRol}");
			}

			// ==========================================
			// Regresar Id
			// ==========================================

			return usuario.Id;
		}

		public async Task<LoginSoporteResponseDTO> LoginAsync(LoginSoporteDTO request, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// Normalizar
			// ==========================================

			var email = request.Email.Trim();

			// ==========================================
			// Buscar usuario
			// ==========================================

			var usuario = await _userManager.FindByEmailAsync(email);

			if (usuario == null)
			{
				throw new BusinessException(
					"Correo electrónico o contraseña incorrectos.");
			}

			// ==========================================
			// Validar contraseña
			// ==========================================

			var passwordCorrecta =
				await _userManager.CheckPasswordAsync(
					usuario,
					request.Password);

			if (!passwordCorrecta)
			{
				throw new BusinessException(
					"Correo electrónico o contraseña incorrectos.");
			}

			// ==========================================
			// Obtener roles
			// ==========================================

			var roles = await _userManager.GetRolesAsync(usuario);

			// ==========================================
			// Validar que sea Soporte
			// ==========================================

			var rolSoporte = roles.FirstOrDefault(
				x => x.Equals(
					"Soporte",
					StringComparison.OrdinalIgnoreCase));

			if (rolSoporte == null)
			{
				throw new BusinessException(
					"El usuario no tiene permisos de soporte.");
			}

			// ==========================================
			// Generar JWT
			// ==========================================

			var jwt = _configuration
				.GetSection("Jwt")
				.Get<JwtSettings>();

			if (jwt == null)
			{
				throw new BusinessException(
					"La configuración JWT no está disponible.");
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, usuario.Id),

				new Claim(ClaimTypes.Name, usuario.UserName ?? usuario.Email ?? string.Empty),

				new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),

				new Claim(ClaimTypes.Role, rolSoporte)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expires = DateTime.UtcNow.AddMinutes(jwt.ExpireMinutes);

			var token = new JwtSecurityToken(
				issuer: jwt.Issuer,
				audience: jwt.Audience,
				claims: claims,
				expires: expires,
				signingCredentials: credentials);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			// ==========================================
			// Respuesta
			// ==========================================

			return new LoginSoporteResponseDTO
			{
				Token = tokenString,
				UserId = usuario.Id,
				Email = usuario.Email ?? string.Empty,
				Role = rolSoporte
			};
		}
	}
}
