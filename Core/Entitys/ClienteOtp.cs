using Core.EntityBase;

namespace Core.Entitys
{
	public class ClienteOtps : BaseEntity
	{
		public int Id { get; set; }
		public Guid ClienteGuidId { get; set; }
		public string UserId { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string CodigoHash { get; set; } = null!;
		public DateTime FechaCreacion { get; set; }
		public DateTime FechaExpiracion { get; set; }
		public DateTime? FechaVerificacion { get; set; }
		public int Intentos { get; set; }
		public bool Usado { get; set; }
		public bool Activo { get; set; }
		public string? Ip { get; set; }
		public DateTime? BloqueadoHasta { get; set; }
	}
}
