namespace Core.DTO.Login.usuarioDispo
{
	public class RegistrarDispositivoRequest
	{
		public string FirebaseToken { get; set; } = null!;

		public string Plataforma { get; set; } = null!;

		public string Modelo { get; set; } = null!;
	}
}
