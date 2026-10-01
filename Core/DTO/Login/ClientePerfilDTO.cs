namespace Core.DTO.Login
{
	public class ClientePerfilDTO
	{
		public Guid GuidId { get; set; }

		public string Nombre { get; set; } = string.Empty;

		public string? FotoPerfil { get; set; }

		public string? Telefono { get; set; }

		public string? Correo { get; set; }
	}
}
