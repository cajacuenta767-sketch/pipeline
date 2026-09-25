using Microsoft.AspNetCore.Http;

namespace Core.DTO.SolocitudCotizaciones
{
	public class RegistrarCotizacionRequest
	{
		public decimal Precio { get; set; }

		public bool Disponible { get; set; }

		public bool EsNueva { get; set; }

		public int MarcaId { get; set; }

		public string? NumeroParte { get; set; }

		public string? Comentarios { get; set; }

		public int? TiempoEntregaDias { get; set; }

		public int DiasGarantia { get; set; }

		public bool EnvioDisponible { get; set; }

		public decimal? CostoEnvio { get; set; }
		public bool	TieneGarantia { get; set; }

		public List<IFormFile>? Imagenes { get; set; }
	}

	public class ImagenCotizacionRequest
	{
		public string UrlImagen { get; set; } = string.Empty;

		public string RutaBlob { get; set; } = string.Empty;

		public bool EsPrincipal { get; set; }

		public int Orden { get; set; }
	}
}
