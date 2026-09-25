using Core.DTO.Solicitudes.Citys;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.ciudades;
using Core.Interfaces.Utilerias;

namespace Core.Services.Requests
{
	public class SolicitudCiudadesService : ISolicitudCiudadesService
	{
		private readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;
		private readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public SolicitudCiudadesService(IUnitOfWorkSolicitudes unitOfWorkSolicitudes, IUnitOfWorkUtilerias unitOfWorkUtilerias)
		{
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;
			_unitOfWorkUtilerias = unitOfWorkUtilerias;
		}

		public async Task AgregarAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken)
		{
			var solicitud = await _unitOfWorkSolicitudes
				.SolicitudesRepository
				.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
			{
				throw new BusinessException("La solicitud no existe.");
			}

			if (solicitud.Cerrada)
			{
				throw new BusinessException("La solicitud ya está cerrada.");
			}

			var ciudad = await _unitOfWorkUtilerias.CiudadesRepository.getCiudadById(ciudadId);

			if (ciudad == null)
			{
				throw new BusinessException("La ciudad no existe.");
			}

			var existe = await _unitOfWorkSolicitudes
				.SolicitudCiudadesRepository
				.ExisteAsync(solicitudGuidId, ciudadId);

			if (existe)
			{
				throw new BusinessException("La ciudad ya está registrada en la solicitud.");
			}

			var entity = new SolicitudesCiudades
			{
				GuidId = Guid.NewGuid(),
				SolicitudGuidId = solicitudGuidId,
				CiudadId = ciudadId
			};


			await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.AgregarAsync(entity);


			await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);
		}

		public async Task AgregarRangoAsync(Guid solicitudGuidId, IList<int> ciudadesIds, CancellationToken cancellationToken)
		{
			if (solicitudGuidId == Guid.Empty)
				throw new BusinessException("El identificador de la solicitud no es válido.");

			if (ciudadesIds == null || !ciudadesIds.Any())
				throw new BusinessException("Debe seleccionar al menos una ciudad.");

			// Eliminar duplicados
			ciudadesIds = ciudadesIds
				.Distinct()
				.ToList();

			// Validar solicitud
			var solicitud = await _unitOfWorkSolicitudes
				.SolicitudesRepository
				.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
				throw new BusinessException("La solicitud no existe.");

			if (solicitud.Cerrada)
				throw new BusinessException("La solicitud ya está cerrada.");

			// Validar ciudades
			var ciudadesValidas = (await _unitOfWorkUtilerias
				.CiudadesRepository
				.getListadoCiudadesId())
				.ToHashSet();

			var ciudadesInvalidas = ciudadesIds
				.Where(x => !ciudadesValidas.Contains(x))
				.ToList();

			if (ciudadesInvalidas.Any())
			{
				throw new BusinessException(
					$"Existen ciudades inválidas: {string.Join(", ", ciudadesInvalidas)}");
			}

			// Obtener ciudades existentes
			var ciudadesExistentes = await _unitOfWorkSolicitudes
				.SolicitudCiudadesRepository
				.ObtenerPorSolicitudAsync(solicitudGuidId);

			var ciudadesExistentesIds = ciudadesExistentes
				.Select(x => x.CiudadId)
				.ToHashSet();

			// Crear únicamente las nuevas
			var nuevasCiudades = ciudadesIds
				.Where(x => !ciudadesExistentesIds.Contains(x))
				.Select(ciudadId => new SolicitudesCiudades
				{
					GuidId = Guid.NewGuid(),
					SolicitudGuidId = solicitudGuidId,
					CiudadId = ciudadId
				})
				.ToList();

			if (!nuevasCiudades.Any())
			{
				throw new BusinessException(
					"No existen ciudades nuevas para agregar a la solicitud.");
			}

			await _unitOfWorkSolicitudes
				.SolicitudCiudadesRepository
				.AgregarRangoAsync(nuevasCiudades);

			await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);

		}

		public async Task<IList<SolicitudesCiudades>> ObtenerPorSolicitudAsync(Guid solicitudGuidId)
		{
			return await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.ObtenerPorSolicitudAsync(solicitudGuidId);
		}

		public async Task<bool> ExisteAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken)
		{
			return await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.ExisteAsync(solicitudGuidId, ciudadId);
		}

		public async Task ActualizarAsync(Guid solicitudGuidId,	IList<int> ciudadesIds, CancellationToken cancellationToken)
		{
			var solicitud = await _unitOfWorkSolicitudes.SolicitudesRepository.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
			{
				throw new BusinessException("La solicitud no existe.");
			}

			if (solicitud.Cerrada)
			{
				throw new BusinessException("La solicitud ya está cerrada.");
			}

			if (ciudadesIds == null || !ciudadesIds.Any())
			{
				throw new BusinessException("Debe seleccionar al menos una ciudad.");
			}

			await using var transaction = await _unitOfWorkSolicitudes.BeginTransactionAsync();

			try
			{
				await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.EliminarPorSolicitudAsync(solicitudGuidId);

				if (ciudadesIds != null && ciudadesIds.Any())
				{
					var nuevasCiudades = ciudadesIds
						.Distinct()
						.Select(x => new SolicitudesCiudades
						{
							GuidId = Guid.NewGuid(),
							SolicitudGuidId = solicitudGuidId,
							CiudadId = x
						}).ToList();

					await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.AgregarRangoAsync(nuevasCiudades);
				}

				await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}


		async Task<bool> ISolicitudCiudadesService.EliminarAsync(Guid solicitudGuidId, int ciudadId, CancellationToken cancellationToken)
		{
			var solicitud = await _unitOfWorkSolicitudes.SolicitudesRepository
				.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
			{
				throw new BusinessException("La solicitud no existe.");
			}

			var entity = await _unitOfWorkSolicitudes.SolicitudCiudadesRepository
				.ObtenerAsync(solicitudGuidId, ciudadId);

			if (entity == null)
			{
				throw new BusinessException("La ciudad no existe en la solicitud.");
			}

			await _unitOfWorkSolicitudes.SolicitudCiudadesRepository
				.EliminarAsync(entity);

			await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);

			return true;
		}

		async Task<bool> ISolicitudCiudadesService.EliminarTodasAsync(Guid solicitudGuidId,	CancellationToken cancellationToken)
		{
			var solicitud = await _unitOfWorkSolicitudes.SolicitudesRepository
				.GetByGuidId(solicitudGuidId);

			if (solicitud == null)
			{
				throw new BusinessException("La solicitud no existe.");
			}

			await _unitOfWorkSolicitudes.SolicitudCiudadesRepository
				.EliminarPorSolicitudAsync(solicitudGuidId);

			await _unitOfWorkSolicitudes.SaveChangesAsync(cancellationToken);

			return true;
		}

		public async Task<IList<SolicitudesCiudades>> CiudadesBySolicitudsync(Guid solicitudGuidId)
		{
			return await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.ObtenerPorSolicitudAsync(solicitudGuidId);
		}

		public async Task<CiudadesListBySolicitud_DTO?> ObtenerCiudadesPorSolicitudAsync(Guid solicitudGuidId)
		{
			return await _unitOfWorkSolicitudes.SolicitudCiudadesRepository.ObtenerCiudadesPorSolicitudAsync(solicitudGuidId);
		}
	}
}
