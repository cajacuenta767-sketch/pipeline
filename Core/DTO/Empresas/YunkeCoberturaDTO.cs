namespace Core.DTO.Empresas
{
	public class YunkeCoberturaDTO
	{
		public Guid YonkeGuidId { get; set; }

		public IList<int> CiudadesIds { get; set; } = new List<int>();
	}
}
