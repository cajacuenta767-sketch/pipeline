using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.Auth.AccesoDetalle;

namespace Core.Services.Soporte.AccesoDetalles
{
	public class AccesoDetalleService : IAccesoDetalleService
	{

		public IUnitOfWorkSoporte _unitOfWorkSoporte;
		public AccesoDetalleService(IUnitOfWorkSoporte unitOfWorkSoporte)
		{
			_unitOfWorkSoporte = unitOfWorkSoporte;
		}


		public async Task<IList<Entitys.AccesoDetalles>> GetAccesoDetalles()
		{
			return (IList<Entitys.AccesoDetalles>)await _unitOfWorkSoporte.AccesoDetalleRepository.GetAllAccesoDetallesAsync();
		}


		public async Task<Entitys.AccesoDetalles> GetAccesoDetalle(int id)
		{
			return await _unitOfWorkSoporte.AccesoDetalleRepository.GetAccesoDetallesById(id);
		}

		

		public async Task InsertAccesoDetalle(Entitys.AccesoDetalles accesoDetalles)
		{
			await _unitOfWorkSoporte.AccesoDetalleRepository.AddAccesoDetalle(accesoDetalles);
			await _unitOfWorkSoporte.SaveChangesAsync();
		}

		public async Task<bool> UpdateAccesoDetalle(Entitys.AccesoDetalles accesoDetalles)
		{
			//reglas de negocio a aplicar
			var existdepen = await _unitOfWorkSoporte.AccesoDetalleRepository.GetAccesoDetallesById(accesoDetalles.Id);
			//envio los campo a modificar de uno en uno
			existdepen.UserId = accesoDetalles.UserId;
			existdepen.EmpresaId = accesoDetalles.EmpresaId;
			existdepen.Estatus = accesoDetalles.Estatus;

			if (existdepen == null)
			{
				throw new BusinessException("Id de AccesoDetalle no Existe");
			}
			//return await _repository.Update(dependencia);
			_unitOfWorkSoporte.AccesoDetalleRepository.UpdateAccesoDetalle(existdepen);
			await _unitOfWorkSoporte.SaveChangesAsync();
			return true;
		}
	}
}
