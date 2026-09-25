using Core.DTO.Solicitudes.imagnees;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Requests.imagenes
{
	public interface ISolicitudesImagenesService
	{
		/// <summary>
		/// Agrega una imagen a una solicitud.
		/// </summary>
		Task<IList<Guid>> AgregarImagenesAsync(Guid solicitudGuidId, IList<IFormFile> imagenes, CancellationToken cancellationToken);

		/// <summary>
		/// Obtiene todas las imágenes de una solicitud.
		/// </summary>
		Task<IList<SolicitudesImagenes_List_DTO>> GetImagenesBySolicitudAsync(Guid solicitudGuidId);

		/// <summary>
		/// Obtiene una imagen por su Guid.
		/// </summary>
		Task<SolicitudesImagenes_DTO?> GetImagenByGuidIdAsync(Guid imagenGuidId);

		/// <summary>
		/// Elimina una imagen.
		/// </summary>
		Task<bool> EliminarImagenAsync(Guid imagenGuidId);
	}
}
