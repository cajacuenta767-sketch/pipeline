using Core.DTO.Empresas;
using Core.Entitys;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.Coberturas;
using Core.Interfaces.Utilerias;

namespace Core.Services.Negocio.Empresa
{
	public class YunkeCoberturaService : IYunkeCoberturaService
	{
		public readonly IUnitOfWorkNegocio _unitOfWork;
		public readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;

		public YunkeCoberturaService(IUnitOfWorkNegocio unitOfWorkNegocio,
									 IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWork = unitOfWorkNegocio;	
			_unitOfWorkUtilerias = unitOfWorkUtilerias;	
		}

		public async Task ActualizarCoberturasAsync(Guid YonkeGuidId, IList<int> ciudadesIds)
		{
			//Si no hay ciudades para grabar
			if (ciudadesIds == null)
				throw new ArgumentNullException(nameof(ciudadesIds));

			//Tener minimo 1 ciudad
			if (!ciudadesIds.Any())
			{
				throw new ArgumentException("Debe seleccionar al menos una ciudad.");
			}

			// Eliminar duplicados
			ciudadesIds = ciudadesIds
				.Distinct()
				.ToList();

			// Obtener los Ids de ciudades existentes
			var ciudadesExistentes = await _unitOfWorkUtilerias
				.CiudadesRepository
				.getListadoCiudadesId();

			// Validar ciudades
			var ciudadesInvalidas = ciudadesIds
				.Except(ciudadesExistentes)
				.ToList();

			if (ciudadesInvalidas.Any())
			{
				throw new ArgumentException($"Las siguientes ciudades no existen: {string.Join(", ", ciudadesInvalidas)}");
			}

			// Obtener yonke
			var yunkeActual = await _unitOfWork
				.YunkeRepository
				.getYunkeGuidById(YonkeGuidId);

			if (yunkeActual == null)
			{
				throw new ArgumentException("El yonke no existe.");
			}

			// Obtener coberturas actuales
			var coberturasActuales = await _unitOfWork
				.YunkeCoberturaRepository
				.ObtenerPorYonkeGuidAsync(YonkeGuidId);

			// HashSet para búsquedas rápidas
			var ciudadesActuales = coberturasActuales?.YunkeHeader?.YunkeCoberturas?
				.Select(x => x.CiudadId)
				.ToHashSet()
				?? new HashSet<int>();

			var ciudadesSeleccionadas = ciudadesIds.ToHashSet();

			// Agregar nuevas coberturas
			var nuevasCoberturas = ciudadesSeleccionadas
				.Except(ciudadesActuales)
				.Select(ciudadId => new YonkesCoberturas
				{
					GuidId = Guid.NewGuid(),
					YonkeGuidId = yunkeActual.GuidId,
					CiudadId = ciudadId,
					Activo = true,
					FechaRegistro = DateTime.UtcNow
				}).ToList();

			if (nuevasCoberturas.Any())
			{
				await _unitOfWork
					.YunkeCoberturaRepository
					.AgregarRangoAsync(nuevasCoberturas);
			}

			var coberturas = coberturasActuales?.YunkeHeader?.YunkeCoberturas
				?? Array.Empty<YunkeCoberturas>();

			var idsEliminar = coberturas
				.Where(x => !ciudadesSeleccionadas.Contains(x.CiudadId))
				.Select(x => x.Id)
				.ToList();

			if (idsEliminar.Any())
			{
				await _unitOfWork.YunkeCoberturaRepository
					.EliminarRangoAsync(idsEliminar);
			}

			await _unitOfWork.SaveChangesAsync();
		}

		public async Task<bool> ExisteAsync(Guid YonkeGuidId, int ciudadId)
		{
			return await _unitOfWork
				   .YunkeCoberturaRepository
				   .ExisteAsync(YonkeGuidId, ciudadId);
		}

		

		public async Task<YonkesCoberturas?> ObtenerAsync(Guid YonkeGuidId, int ciudadId)
		{
			return await _unitOfWork
					  .YunkeCoberturaRepository
					  .ObtenerAsync(YonkeGuidId, ciudadId);
	    }

		

		public async Task<IList<YonkesCoberturas>> ObtenerPorYonkeAsync(Guid YonkeGuidId)
		{
			return await _unitOfWork
				   .YunkeCoberturaRepository
				   .ObtenerPorYonkeAsync(YonkeGuidId);
		}

		

		public async Task<YunkeWithCoberturasDTO?> ObtenerPorYonkeGuidAsync(Guid YonkeGuidId)
		{
			return await _unitOfWork
				.YunkeCoberturaRepository
				.ObtenerPorYonkeGuidAsync(YonkeGuidId);
		}

		
	}
}
