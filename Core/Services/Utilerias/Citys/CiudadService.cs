using Core.DTO.Utilerias.Ciudades;
using Core.Entitys;
using Core.Interfaces.Utilerias;
using Core.Interfaces.Utilerias.Citys;

namespace Core.Services.Utilerias.Citys
{
    public class CiudadService : ICiudadService
    {
		public readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public CiudadService(IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWorkUtilerias = unitOfWorkUtilerias;
		}

		public async Task<IList<CiudadesListDTO>> getCiudades(int entidadId)
		{
			return await _unitOfWorkUtilerias.CiudadesRepository.getCiudadesList(entidadId);
		}

		public async Task<IList<int>> getCiudadesListadoId()
		{
			return await _unitOfWorkUtilerias.CiudadesRepository.getListadoCiudadesId();
		}


		public async Task<Ciudades> getCiudadById(int id)
		{
			return await _unitOfWorkUtilerias.CiudadesRepository.getCiudadById(id);
		}

	}
}
