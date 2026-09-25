using System.ComponentModel.DataAnnotations;

namespace Core.DTO.BuildSecurity
{
	public class AccesoDetalleDTO
	{
		//public int Id { get; set; }
		[Required]
		public string UserId { get; set; }
		[Required]
		public int EmpresaId { get; set; }
		[Required]
		public bool Estatus { get; set; }
	}
}
