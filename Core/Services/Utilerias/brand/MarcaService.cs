using Core.DTO.Utilerias.Marcas;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Utilerias;
using Core.Interfaces.Utilerias.brand;

namespace Core.Services.Utilerias.brand
{
	public class MarcaService : IMarcaService
	{
		private readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public MarcaService(IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWorkUtilerias = unitOfWorkUtilerias;
		}


		public async Task<IList<MarcasListDTO>> getMarcasListByEmpresa()
		{
			return await _unitOfWorkUtilerias.MarcasRepository.getMarcas();
		}

		public async Task<Marcas> getMarcaById(int id)
		{
			return await _unitOfWorkUtilerias.MarcasRepository.getMarcaById(id);
		}

		public async Task NuevoMarca(Marcas marcaSave)
		{
			await _unitOfWorkUtilerias.MarcasRepository.addMarca(marcaSave);
			await _unitOfWorkUtilerias.SaveChangesAsync();
		}

		public async Task<bool> UpdateMarca(Marcas marcaUpdate)
		{
			// 1️⃣ Obtener cliente con contactos
			var clienteDb = await _unitOfWorkUtilerias.MarcasRepository
				.getMarcaById(marcaUpdate.Id); // 👈 Debe traer Include

			if (clienteDb == null)
			{
				throw new BusinessException(
					$"El cliente con id '{marcaUpdate.Id}' no existe", 404);
			}

			// 2️⃣ Actualizar datos del cliente
			clienteDb.Marca = marcaUpdate.Marca;

			await _unitOfWorkUtilerias.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExisteMarca(int id)
		{
			var data = await _unitOfWorkUtilerias.MarcasRepository.getMarcaById(id);
			return data != null;
		}

		

		

		

		
	}
}
