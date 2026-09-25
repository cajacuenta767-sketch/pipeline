namespace Core.Entitys
{
	public class Clientes
	{
		public int Id { get; set; }
		public DateTime CreateAt { get; set; } = DateTime.UtcNow;
		public Guid GuidId { get; set; } = Guid.NewGuid();
		public string Nombre { get; set; }
		public string? Correo { get; set; }
		public string? GoogleId { get; set; }
		public string? AppleId { get; set; }
		public bool Activo { get; set; }
		public string? FotoPerfil { get; set; }
		public DateTime FechaRegistro { get; set; }


		public string? Telefono { get; set; }
		public bool TelefonoConfirmado { get; set; }


	}
}
