using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Fijas;
using Core.Models.BuildSecurity;
using Infra.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using static Infra.Utilerias.GenerarPassword;

namespace Infra.Repositorys.SeguridadUser
{
	public class UserService : IUserService
	{
		private UserManager<IdentityUser> _userManger;
		private IConfiguration _configuration;
		private IMailService _mailService;
		private readonly RoleManager<IdentityRole> _roleManager;

		private AplicationDBContext _context;

		public UserService(UserManager<IdentityUser> userManager,
									  IConfiguration configuration,
									  IMailService mailService,
									  RoleManager<IdentityRole> roleManager,
									  AplicationDBContext context)
		{
			_userManger = userManager;
			_configuration = configuration;
			_mailService = mailService;

			_roleManager = roleManager;

			_context = context;

		}


		#region Registro de usuario (Asociado)

		#endregion
		public async Task<UserManagerResponse> RegisterAsociadoAsync(RegisterViewAspociadoModel model)
		{
			#region Set up Role if doesn't exist
			if (!await _roleManager.RoleExistsAsync(model.Role))
			{
				await _roleManager.CreateAsync(new IdentityRole(model.Role));
			}
			#endregion

			#region Modelo Vacio
			if (model == null)
				throw new NullReferenceException("Register Model is null");
			#endregion



			var identityUser = new IdentityUser
			{
				Email = model.Email,
				UserName = model.Email,
				//PasswordHash = model.Password,
				PhoneNumber = model.PhoneNumber,
				PhoneNumberConfirmed = false,
				EmailConfirmed = true
			};


			//Generar password de acceso
			//int length = 10;
			//string passwordSend = PasswordGenerate.GetRandomPassword(length);
			//var passGeneradoAutomatic = passwordSend;

			#region Create User
			var result = await _userManger.CreateAsync(identityUser, model.Password);
			#endregion

			var iduserCreado = "";
			if (result.Succeeded)
			{
				//busca el usuario recien creado
				var userFromDb = await _userManger.FindByNameAsync(identityUser.UserName);
				iduserCreado = userFromDb.Id;

				//Send Confirmation Email
				var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(identityUser);

				var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
				var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

				//string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

				//string urlSistema = "https://siifai.azurewebsites.net/auth/login";

				await _mailService.SendEmailAsync(identityUser.Email, "Confirmación de acceso", $"<h1>Bienvenido a Yonke App</h1>" );


				//Add user to role
				await _userManger.AddToRoleAsync(userFromDb, model.Role);

			
				return new UserManagerResponse
				{
					Message = "Registro exitoso!",
					IsSuccess = true,
				};
			}

			return new UserManagerResponse
			{
				Message = "Usuario no creado",
				IsSuccess = false,
				Errors = result.Errors.Select(e => e.Description)
			};



		}
	}
}
