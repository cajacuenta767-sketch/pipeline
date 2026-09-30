using Core.DTO.Login;
using Core.Exceptions;
using Core.Interfaces.Login.Yonke;
using Core.Models.BuildSecurity;
using Core.ResponseGlobal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

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

		// =========================================================
		// LOGIN
		// =========================================================

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginYonkeRequest request)
		{
			try
			{
				var response =
					await _service.LoginAsync(request);

				return Ok(response);
			}
			catch (BusinessException ex)
			{
				return BadRequest(
					ApiResponseGlobal<string>.Fail(ex.Message));
			}
			catch (Exception)
			{
				return StatusCode(
					500,
					ApiResponseGlobal<string>.Fail(
						"Error al iniciar sesión."));
			}
		}


		// =========================================================
		// FORGOT PASSWORD
		// =========================================================

		[HttpPost("forgot-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
		{
			// Siempre devolvemos la misma respuesta.
			// No revelamos si el correo existe.

			try
			{
				await _service.ForgetPasswordAsync(
					request.Email);
			}
			catch
			{
				// No revelar información del usuario.
			}

			return Ok(
				ApiResponseGlobal<string>.Ok(
					string.Empty,
					"Si el correo está registrado, recibirás instrucciones para recuperar tu contraseña."));
		}


		// =========================================================
		// RESET PASSWORD
		// =========================================================

		[HttpPost("reset-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
		{
			var response =
				await _service.ResetPasswordAsync(model);

			if (!response.IsSuccess)
			{
				return BadRequest(new { isSuccess = false, message = response.Message });
			}

			return Ok(new
			{
				isSuccess = true,
				message = "¡Contraseña actualizada correctamente!"
			});
		}


		// =========================================================
		// PÁGINA HTML PARA CAMBIAR PASSWORD
		// =========================================================

		[HttpGet("reset-password-page")]
		[AllowAnonymous]
		public IActionResult ResetPasswordPage([FromQuery] string email, [FromQuery] string token)
		{
			if (string.IsNullOrWhiteSpace(email) ||
				string.IsNullOrWhiteSpace(token))
			{
				return Content(
					GenerarPaginaError(
						"El enlace de recuperación no es válido."),
					"text/html");
			}


			// =========================================================
			// SERIALIZAR EMAIL Y TOKEN
			// =========================================================

			var emailJson =
				JsonSerializer.Serialize(email);

			var tokenJson =
				JsonSerializer.Serialize(token);


			// =========================================================
			// HTML
			// =========================================================

			var html = $@"
<!DOCTYPE html>

<html lang='es'>

<head>

	<meta charset='UTF-8'>

	<meta name='viewport'
		  content='width=device-width, initial-scale=1.0'>

	<title>Restablecer contraseña - RefaNet</title>


	<style>

		* {{
			box-sizing: border-box;
		}}

		body {{
			margin: 0;

			font-family: Arial, sans-serif;

			background: #f4f6f8;

			display: flex;

			justify-content: center;

			align-items: center;

			min-height: 100vh;
		}}

		.container {{
			width: 100%;

			max-width: 430px;

			background: white;

			padding: 35px;

			border-radius: 14px;

			box-shadow:
				0 5px 25px rgba(0,0,0,.10);
		}}

		.logo {{
				text-align: center;

				margin-bottom: 20px;
			}}

			.logo img {{
				width: 180px;
				max-width: 80%;
				height: auto;
				display: inline-block;
			}}

		h2 {{
			text-align: center;

			margin-bottom: 10px;
		}}

		.description {{
			text-align: center;

			color: #666;

			margin-bottom: 25px;
		}}

		label {{
			display: block;

			margin-bottom: 7px;

			font-weight: bold;
		}}

		input {{
			width: 100%;

			padding: 13px;

			border: 1px solid #ccc;

			border-radius: 7px;

			margin-bottom: 18px;

			font-size: 16px;
		}}

		input:focus {{
			outline: none;

			border-color: #2563eb;

			box-shadow:
				0 0 0 2px rgba(37,99,235,.10);
		}}

		button {{
			width: 100%;

			padding: 13px;

			border: none;

			border-radius: 7px;

			background: #2563eb;

			color: white;

			font-size: 16px;

			cursor: pointer;
		}}

		button:hover {{
			background: #1d4ed8;
		}}

		button:disabled {{
			background: #93c5fd;

			cursor: not-allowed;
		}}

		.requirements {{
			font-size: 13px;

			color: #666;

			margin-top: -10px;

			margin-bottom: 18px;
		}}

		.message {{
			margin-top: 20px;

			padding: 13px;

			border-radius: 7px;

			display: none;

			text-align: center;
		}}

		.success {{
			background: #dcfce7;

			color: #166534;
		}}

		.error {{
			background: #fee2e2;

			color: #991b1b;
		}}

		#successContainer {{
			display: none;

			text-align: center;
		}}

		.success-icon {{
			font-size: 50px;

			margin-bottom: 10px;
		}}

	</style>

</head>


<body>


<div class='container'>

	<div class='logo'>
		<img
			src='https://yonkeimagenes.blob.core.windows.net/refanet/icono02.png'
			alt='RefaNet'
		/>
	</div>


	<div id='formContainer'>

		<h2>
			Restablecer contraseña
		</h2>


		<div class='description'>
			Ingresa tu nueva contraseña.
		</div>


		<form id='resetForm'>

			<label>
				Nueva contraseña
			</label>

			<input
				type='password'
				id='newPassword'
				required
				minlength='8'
				autocomplete='new-password'
			/>


			<div class='requirements'>
				Mínimo 8 caracteres.
			</div>


			<label>
				Confirmar contraseña
			</label>

			<input
				type='password'
				id='confirmPassword'
				required
				minlength='8'
				autocomplete='new-password'
			/>


			<button
				type='submit'
				id='resetButton'>

				Cambiar contraseña

			</button>

		</form>


		<div
			id='message'
			class='message'>
		</div>

	</div>


	<div id='successContainer'>

		<div class='success-icon'>
			✓
		</div>

		<h2>
			¡Contraseña actualizada!
		</h2>

		<p>
			Tu contraseña fue actualizada correctamente.
		</p>

		<p>
			Ya puedes cerrar esta página e iniciar sesión
			nuevamente en RefaNet.
		</p>

	</div>

</div>


<script>

	// =========================================================
	// EMAIL Y TOKEN RECIBIDOS DESDE C#
	// =========================================================

	const email = {emailJson};

	const token = {tokenJson};


	console.log('====================================');
	console.log('RESET PASSWORD PAGE');
	console.log('====================================');

	console.log('EMAIL:', email);

	console.log('TOKEN:', token);

	console.log('EMAIL EXISTE:', !!email);

	console.log('TOKEN EXISTE:', !!token);


	// =========================================================
	// ELEMENTOS HTML
	// =========================================================

	const form =
		document.getElementById('resetForm');

	const message =
		document.getElementById('message');

	const resetButton =
		document.getElementById('resetButton');

	const formContainer =
		document.getElementById('formContainer');

	const successContainer =
		document.getElementById('successContainer');


	// =========================================================
	// VALIDAR EMAIL Y TOKEN
	// =========================================================

	if (!email || !token) {{

		showMessage(
			'El enlace de recuperación no es válido.',
			false
		);

		resetButton.disabled = true;
	}}


	// =========================================================
	// SUBMIT
	// =========================================================

	form.addEventListener(
		'submit',
		async function(e) {{

			e.preventDefault();


			console.log('====================================');

			console.log('SUBMIT EJECUTADO');

			console.log('====================================');


			const newPassword =
				document.getElementById(
					'newPassword'
				).value;


			const confirmPassword =
				document.getElementById(
					'confirmPassword'
				).value;


			// =================================================
			// VALIDAR PASSWORD
			// =================================================

			if (newPassword.length < 8) {{

				showMessage(
					'La contraseña debe tener al menos 8 caracteres.',
					false
				);

				return;
			}}


			if (newPassword !== confirmPassword) {{

				showMessage(
					'Las contraseñas no coinciden.',
					false
				);

				return;
			}}


			// =================================================
			// DESHABILITAR BOTÓN
			// =================================================

			resetButton.disabled = true;

			resetButton.innerText =
				'Actualizando...';


			// =================================================
			// CREAR BODY
			// =================================================

			const body = {{
				email: email,
				token: token,
				newPassword: newPassword,
				confirmPassword: confirmPassword
			}};


			console.log('====================================');

			console.log('BODY QUE SE ENVÍA A LA API');

			console.log('====================================');

			console.log(
				JSON.stringify(body)
			);


			try {{

				// =================================================
				// POST API
				// =================================================

				const response =
					await fetch(
						'/api/YonkeAuth/reset-password',
						{{
							method: 'POST',

							headers: {{
								'Content-Type':
									'application/json',

								'Accept':
									'application/json'
							}},

							body:
								JSON.stringify(body)
						}}
					);


				console.log(
					'STATUS HTTP:',
					response.status
				);


				// =================================================
				// LEER RESPUESTA
				// =================================================

				const responseText =
					await response.text();


				console.log(
					'RESPUESTA API:',
					responseText
				);


				let data = null;


				try {{

					data =
						JSON.parse(responseText);

				}}
				catch {{

					console.log(
						'La respuesta no es JSON.'
					);

				}}


				// =================================================
				// ÉXITO
				// =================================================

				if (
					response.ok &&
					data &&
					data.isSuccess
				) {{

					formContainer.style.display =
						'none';

					successContainer.style.display =
						'block';

					return;
				}}


				// =================================================
				// ERROR
				// =================================================

				let mensaje =
					'No fue posible cambiar la contraseña.';


				if (data && data.message) {{

					mensaje =
						data.message;

				}}


				if (
					data &&
					data.errors
				) {{

					const errores =
						Object.values(data.errors)
							.flat()
							.join('<br>');

					mensaje =
						errores;
				}}


				showMessage(
					mensaje,
					false
				);


			}}
			catch (error) {{

				console.error(
					'ERROR FETCH:',
					error
				);


				showMessage(
					'Ocurrió un error al procesar la solicitud.',
					false
				);

			}}
			finally {{

				resetButton.disabled =
					false;

				resetButton.innerText =
					'Cambiar contraseña';

			}}

		}}
	);


	// =========================================================
	// MOSTRAR MENSAJE
	// =========================================================

	function showMessage(text, success) {{

		message.innerText =
			text;


		message.className =
			'message ' +
			(
				success
					? 'success'
					: 'error'
			);


		message.style.display =
			'block';

	}}

</script>


</body>

</html>
";


			return Content(
				html,
				"text/html");
		}


		// =========================================================
		// PÁGINA DE ERROR
		// =========================================================

		private string GenerarPaginaError(string mensaje)
		{
			return $@"
			<!DOCTYPE html>

			<html lang='es'>

			<head>

				<meta charset='UTF-8'>

				<meta name='viewport'
					  content='width=device-width, initial-scale=1.0'>

				<title>RefaNet</title>

				<style>

					body {{
						font-family: Arial, sans-serif;

						background: #f4f6f8;

						display: flex;

						justify-content: center;

						align-items: center;

						min-height: 100vh;
					}}

					.container {{
						background: white;

						padding: 35px;

						border-radius: 12px;

						text-align: center;

						max-width: 420px;

						box-shadow:
							0 5px 25px rgba(0,0,0,.10);
					}}

					h2 {{
						color: #991b1b;
					}}

					p {{
						color: #666;
					}}

				</style>

			</head>

			<body>

				<div class='container'>

					<h2>
						Enlace inválido
					</h2>

					<p>
						{System.Net.WebUtility.HtmlEncode(mensaje)}
					</p>

					<p>
						Solicita nuevamente la recuperación
						de tu contraseña desde RefaNet.
					</p>

				</div>

			</body>

			</html>";
		}



	}
}



