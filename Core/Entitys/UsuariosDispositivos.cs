using Core.EntityBase;

namespace Core.Entitys
{
	public class UsuariosDispositivos : BaseEntity
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; } = Guid.NewGuid();

		// AspNetUsers.Id
		public string UsuarioId { get; set; } = null!;

		// Token FCM del dispositivo
		public string FirebaseToken { get; set; } = null!;

		// Android / iOS
		public string Plataforma { get; set; } = null!;

		// Ejemplo: Samsung SM-S928B / iPhone 15
		public string Modelo { get; set; } = null!;

		public bool Activo { get; set; }

		public DateTime FechaRegistro { get; set; }

		public DateTime? UltimoAcceso { get; set; }
	}
}
