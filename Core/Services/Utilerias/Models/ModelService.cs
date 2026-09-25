using Core.DTO.Utilerias.Models;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Utilerias;
using Core.Interfaces.Utilerias.Models;

namespace Core.Services.Utilerias.Models
{
	public class ModelService : IModeloService
	{
		public readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public ModelService(IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWorkUtilerias = unitOfWorkUtilerias;	
		}

		public async Task<IList<ModelosListDTO>> getModelosListByEmpresa(int marcaId)
		{
			return await _unitOfWorkUtilerias.ModelosRepository.getModelos(marcaId);
		}

		public async Task<Modelos> getModeloById(int id)
		{
			return await _unitOfWorkUtilerias.ModelosRepository.getModeloById(id);
		}

		public async Task NuevoModelo(Modelos modeloSave)
		{
			await _unitOfWorkUtilerias.ModelosRepository.addModelo(modeloSave);
			await _unitOfWorkUtilerias.SaveChangesAsync();
		}


		public async Task<bool> UpdateModelo(Modelos modeloUpdate)
		{
			// 1️⃣ Obtener cliente con contactos
			var clienteDb = await _unitOfWorkUtilerias.ModelosRepository
				.getModeloById(modeloUpdate.Id); // 👈 Debe traer Include

			if (clienteDb == null)
			{
				throw new BusinessException(
					$"El modelo con id '{modeloUpdate.Id}' no existe", 404);
			}

			// 2️⃣ Actualizar datos del cliente
			clienteDb.Modelo = modeloUpdate.Modelo;
			clienteDb.MarcaId = modeloUpdate.MarcaId;	

			await _unitOfWorkUtilerias.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExisteModelo(int id)
		{
			var data = await _unitOfWorkUtilerias.ModelosRepository.getModeloById(id);
			return data != null;
		}
	}
}
