namespace Core.Models.BuildSecurity
{
	public class UserTokenResponse
	{
		public string? Token { get; set; }
		public bool IsSuccess { get; set; }
		public IEnumerable<string>? Errors { get; set; }
		public DateTime? ExpireDate { get; set; }

		public IEnumerable<string>? Permisos { get; set; }
		public string? Usuario { get; set; }
		public string? UserId { get; set; }

		//public string? PhoneNumber { get; set; }

		public int EmpresaId { get; set; }
		public string? EmpresaNombre { get; set; }
	}
}
