using Core.DTO.Solicitudes.imagnees;
using Core.EntityBase;
using Core.Entitys;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Requests
{
	public interface ISolicitudImagenesRepository<T> where T : BaseEntity
	{
		//Task<Guid> AgregarImagenAsync(Guid solicitudGuidId, IFormFile imagen);

		//Listado de 3 imagenes
		//Task<IList<Guid>> AgregarImagenesAsync(Guid solicitudGuidId, IList<IFormFile> imagenes);

		Task AgregarImagenAsync(SolicitudesImagenes entity);

		Task<IList<SolicitudesImagenes_List_DTO>> GetImagenesBySolicitudAsync(Guid solicitudGuidId);

		Task<SolicitudesImagenes_DTO?> GetImagenByGuidIdAsync(Guid imagenGuidId);
		Task<SolicitudesImagenes?> GetByGuidId(Guid guidId);

		Task<bool> EliminarImagenAsync(Guid imagenGuidId);
	}
}
