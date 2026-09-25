using Core.Interfaces.Login.ClienteOtp;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace Core.Services.TwilioSms
{
	public class TwilioSmsService : ISmsService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public TwilioSmsService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
		}

		public async Task<bool> EnviarSmsAsync(
	   string telefono,
	   string mensaje,
	   CancellationToken cancellationToken = default)
		{
			var accountSid =
				_configuration["Twilio:AccountSid"];

			var authToken =
				_configuration["Twilio:AuthToken"];

			var from =
				_configuration["Twilio:From"];

			if (string.IsNullOrWhiteSpace(accountSid))
				throw new InvalidOperationException(
					"Twilio:AccountSid no está configurado.");

			if (string.IsNullOrWhiteSpace(authToken))
				throw new InvalidOperationException(
					"Twilio:AuthToken no está configurado.");

			if (string.IsNullOrWhiteSpace(from))
				throw new InvalidOperationException(
					"Twilio:From no está configurado.");

			if (string.IsNullOrWhiteSpace(telefono))
				throw new ArgumentException(
					"El teléfono es obligatorio.",
					nameof(telefono));

			if (string.IsNullOrWhiteSpace(mensaje))
				throw new ArgumentException(
					"El mensaje es obligatorio.",
					nameof(mensaje));

			var url =
				$"https://api.twilio.com/2010-04-01/" +
				$"Accounts/{accountSid}/Messages.json";

			var content = new FormUrlEncodedContent(
				new Dictionary<string, string>
				{
					["To"] = telefono,
					["From"] = from,
					["Body"] = mensaje
				});

			var credentials =
				Convert.ToBase64String(
					Encoding.ASCII.GetBytes(
						$"{accountSid}:{authToken}"));

			using var request = new HttpRequestMessage(
				HttpMethod.Post,
				url);

			request.Headers.Authorization =
				new AuthenticationHeaderValue(
					"Basic",
					credentials);

			request.Content = content;

			var response =
				await _httpClient.SendAsync(
					request,
					cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				var error =
					await response.Content.ReadAsStringAsync(
						cancellationToken);

				throw new InvalidOperationException(
					$"Error enviando SMS por Twilio: {error}");
			}

			return true;
		}
	}
}
