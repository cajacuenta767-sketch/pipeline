namespace Core.Pagination
{
	public class PaginacionDTO
	{
		public int Page { get; set; } = 1;

		public string? Search { get; set; }

		private int cantidadRegistrosPorPagina = 10;
		private readonly int cantidadMaximaRegistrosPorPagina = 50;

		public int CantidadRegistrosPorPagina
		{

			get => cantidadRegistrosPorPagina;
			set
			{
				cantidadRegistrosPorPagina = (value > cantidadMaximaRegistrosPorPagina) ? cantidadMaximaRegistrosPorPagina : value;
			}

		}
	}
}
