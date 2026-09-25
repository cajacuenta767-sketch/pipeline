namespace Core.DTO.Login.smsMasicos
{
	using System.Text.Json.Serialization;

	public class SmsMasivosRequest
	{
		[JsonPropertyName("phone_number")]
		public string PhoneNumber { get; set; } = string.Empty;

		[JsonPropertyName("country_code")]
		public string CountryCode { get; set; } = "52";

		[JsonPropertyName("company")]
		public string Company { get; set; } = "refaNet";

		[JsonPropertyName("message")]
		public string Message { get; set; } = string.Empty;
	}
}
