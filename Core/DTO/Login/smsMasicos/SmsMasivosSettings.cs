namespace Core.DTO.Login.smsMasicos
{
	namespace Core.Configuration
	{
		public class SmsMasivosSettings
		{
			public string ApiKey { get; set; } = string.Empty;
			public string BaseUrl { get; set; } = "https://api.smsmasivos.com.mx";
			public string Sender { get; set; } = "refaNet";
		}
	}
}
