using Core.DTO.Login.smsMasicos;
using Core.DTO.Login.smsMasicos.Core.Configuration;
using Core.Interfaces.Login.ClienteOtp;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Core.Services.smsMasivo
{
	public class SmsMasivosService : ISmsService
	{
		private readonly HttpClient _httpClient;
		private readonly SmsMasivosSettings _settings;

		public SmsMasivosService(HttpClient httpClient, IOptions<SmsMasivosSettings> options)
		{
			_httpClient = httpClient;
			_settings = options.Value;
		}
		public async Task<bool> EnviarSmsAsync(string telefono, string mensaje, CancellationToken cancellationToken = default)
		{
			var numero = NormalizarTelefono(telefono);

			var request = new
			{
				numbers = numero,
				message = mensaje,
				country_code = "52"
			};

			using var requestMessage = new HttpRequestMessage(
				HttpMethod.Post,
				"/sms/send");

			requestMessage.Headers.Add(
				"apikey",
				_settings.ApiKey);

			requestMessage.Content =
				JsonContent.Create(request);

			var response = await _httpClient.SendAsync(
				requestMessage,
				cancellationToken);

			var responseBody =
				await response.Content.ReadAsStringAsync(
					cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException(
					$"SMS Masivos HTTP {(int)response.StatusCode}: {responseBody}");
			}

			using var json = JsonDocument.Parse(responseBody);

			var root = json.RootElement;

			if (root.TryGetProperty("success", out var successProperty))
			{
				var success = successProperty.GetBoolean();

				if (!success)
				{
					var message =
						root.TryGetProperty("message", out var messageProperty)
							? messageProperty.GetString()
							: "Error desconocido de SMS Masivos.";

					var code =
						root.TryGetProperty("code", out var codeProperty)
							? codeProperty.GetString()
							: null;

					throw new InvalidOperationException(
						$"SMS Masivos rechazó el envío: " +
						$"{message}" +
						$"{(string.IsNullOrWhiteSpace(code) ? "" : $" Código: {code}")}");
				}

				// SMS enviado correctamente
				return true;
			}

			// Si HTTP fue 200 pero no encontramos success
			throw new InvalidOperationException(
				$"Respuesta inesperada de SMS Masivos: {responseBody}");

		
		}


		private static string NormalizarTelefono(string telefono)
		{
			if (string.IsNullOrWhiteSpace(telefono))
			{
				throw new ArgumentException(
					"El teléfono es obligatorio.",
					nameof(telefono));
			}

			telefono = telefono.Trim();

			// +526311188509
			if (telefono.StartsWith("+52"))
			{
				telefono = telefono.Substring(3);
			}

			// 526311188509
			else if (telefono.StartsWith("52") &&
					 telefono.Length == 12)
			{
				telefono = telefono.Substring(2);
			}

			// 6311188509
			if (telefono.Length != 10 ||
				!telefono.All(char.IsDigit))
			{
				throw new ArgumentException(
					$"Número de teléfono inválido: {telefono}");
			}

			return telefono;

		}
	}


}
