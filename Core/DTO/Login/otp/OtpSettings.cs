namespace Core.DTO.Login.otp
{
	public class OtpSettings
	{
		public int Longitud { get; set; } = 6;
		public int ExpiracionMinutos { get; set; } = 5;
		public int MaxIntentos { get; set; } = 5;
		public int MinutosEntreSolicitudes { get; set; } = 1;
	}
}
