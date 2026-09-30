using AutoMapper;
using Core.DTO.Solicitudes.Requests;
using Core.DTO.SolocitudCotizaciones;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Auth;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.ciudades;
using Core.Interfaces.Requests.Solicitud;
using Core.Interfaces.RequestYonkes;

namespace Core.Services.Requests
{
	public class SolicitudService : ISolicitudService
	{
		public readonly IUnitOfWorkSolicitudes _unitofWorkSolicitudes;
		public readonly IUnitOfWorkSoporte _unitOfWorkSoporte;
		public readonly IUnitOfWorkSolicitudYonkes _unitOfWorkSolicitudYonkes;
		public readonly IMapper _mapper;
		//private readonly IHttpContextAccessor _httpContextAccessor;

		public readonly ICurrentUserService _currentUserService;


		public readonly ISolicitudCiudadesService _solicitudCiudadesService;

		public SolicitudService(IUnitOfWorkSolicitudes unitOfWorkSolicitudes,
								IUnitOfWorkSoporte unitOfWorkSoporte,
								IUnitOfWorkSolicitudYonkes unitOfWorkSolicitudYonkes,
							    IMapper mapper,
								ICurrentUserService currentUserService,
								ISolicitudCiudadesService solicitudCiudadesService)
		{
			_unitofWorkSolicitudes = unitOfWorkSolicitudes;
			_unitOfWorkSoporte = unitOfWorkSoporte;
			_unitOfWorkSolicitudYonkes = unitOfWorkSolicitudYonkes;
			_mapper = mapper;
			_currentUserService = currentUserService;

			_solicitudCiudadesService = solicitudCiudadesService;
		}

		public IQueryable<Solicitud_Busqueda_DTO> GetSolicitudesByUserId(DateTime desde, DateTime hasta, Guid userId)
		{
			return _unitofWorkSolicitudes.SolicitudesRepository.GetSolicitudesByUserId(desde, hasta, userId);
		}

		public async Task<Solicitud_Busqueda_DTO> GetSolicitudByGuidId(Guid guidId )
		{
			return await _unitofWorkSolicitudes.SolicitudesRepository.GetSolicitudByGuidId(guidId);
		}

		public async Task<Solicitudes?> GetByGuidId(Guid guidId)
		{
			return await _unitofWorkSolicitudes.SolicitudesRepository.GetByGuidId(guidId);
		}

		public async Task<Solicitudes> NuevaSolicitud(Solicitudes_Create_DTO solicitudSave,	CancellationToken cancellationToken)
		{
			// ======================================
			// Validaciones
			// ======================================
			if (solicitudSave == null)
				throw new ArgumentNullException(nameof(solicitudSave));

			if (solicitudSave.CiudadesIds == null || !solicitudSave.CiudadesIds.Any())
				throw new BusinessException("Debe seleccionar al menos una ciudad.");

			// ======================================
			// Validar límite de solicitudes diarias
			// ======================================

			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			var inicioDia = DateTime.UtcNow.Date;
			var finDia = inicioDia.AddDays(1);

			var solicitudesHoy = await _unitofWorkSolicitudes
				.SolicitudesRepository
				.CountAsync(x =>
					x.UsuarioId == usuarioId &&
					x.FechaCreacion >= inicioDia &&
					x.FechaCreacion < finDia,
					cancellationToken);

			//***********Modificar cuando se tenga las membresias de los clientes ****************///
			const int limiteSolicitudesDiarias = 10;

			if (solicitudesHoy >= limiteSolicitudesDiarias)
			{
				throw new BusinessException(
					"Has alcanzado el límite de 3 solicitudes de cotización por día.");
			}


			// Eliminar ciudades duplicadas
			var ciudadesIds = solicitudSave.CiudadesIds
				.Distinct()
				.ToList();

			// ======================================
			// Mapeo
			// ======================================
			var entity = _mapper.Map<Solicitudes>(solicitudSave);

			// ======================================
			// Datos generados por el servidor
			// ======================================

			entity.Id = 0;
			entity.GuidId = Guid.NewGuid();
			entity.FechaCreacion = DateTime.UtcNow;
			entity.EstatusSolicitudId = (int)EstatusSolicitudEnum.Pendiente;
			entity.Cerrada = false;
			entity.UsuarioId = usuarioId.Value;

			// ======================================
			// Sumar 15 dias para fecha de cierre
			// ======================================
			entity.FechaCierre = DateTime.UtcNow.AddDays(15);


			// Generar folio consecutivo
			entity.Folio = await _unitofWorkSolicitudes
				.SolicitudesRepository
				.GenerarFolioSolicitudAsync();

			// ======================================
			// Transacción
			// ======================================
			await using var transaction = await _unitofWorkSolicitudes.BeginTransactionAsync();

			try
			{
				// ======================================
				// Guardar solicitud
				// ======================================
				await _unitofWorkSolicitudes
					.SolicitudesRepository
					.AddAsync(entity, cancellationToken);

				// Necesario para obtener el Identity (entity.Id)
				await _unitofWorkSolicitudes.SaveChangesAsync();

				// ======================================
				// Guardar ciudades
				// ======================================
				await _solicitudCiudadesService.AgregarRangoAsync(
					entity.GuidId,
					ciudadesIds,
					cancellationToken);

				// ======================================
				// Guardar historial
				// ======================================
				var historial = new SolicitudesHistorials
				{
					SolicitudGuidId = entity.GuidId,
					EstatusSolicitudId = (int)EstatusSolicitudEnum.Pendiente,
					Fecha = DateTime.UtcNow,
					UsuarioId = entity.UsuarioId,
					Comentarios = "Solicitud creada."
				};

				await _unitofWorkSolicitudes
					.solicitudHistorialRepository
					.AddHistorialAsync(historial);

				// ======================================
				// Guardar cambios finales
				// ======================================
				await _unitofWorkSolicitudes.SaveChangesAsync();

				// ======================================
				// Confirmar transacción
				// ======================================
				await transaction.CommitAsync();

				return entity;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<bool> UpdateSolcitud(Solicitudes solicitud)
		{
			//reglas de negocio a aplicar
			var existdepen = await _unitofWorkSolicitudes.SolicitudesRepository.GetByGuidId(solicitud.GuidId);
				

			//envio los campo a modificar de uno en uno		
			existdepen.MarcaId = solicitud.MarcaId;
			existdepen.ModeloId = solicitud.ModeloId;
			existdepen.Año = solicitud.Año;
			existdepen.Motor = solicitud.Motor;
			existdepen.Transmicion = solicitud.Transmicion;
			existdepen.PiezaBuscada = solicitud.PiezaBuscada;
			existdepen.NumeroParte = solicitud.NumeroParte;
			existdepen.Descripcion = solicitud.Descripcion;


			if (existdepen == null)
			{
				throw new BusinessException("Id de yunke no Existe");
			}

			//return await _repository.Update(dependencia);
			_unitofWorkSolicitudes.SolicitudesRepository.UpdateSolicitud(existdepen);
			await _unitofWorkSolicitudes.SaveChangesAsync();

			return true;
		}


		/// <summary>
		/// Cancelar Solicitud con historial y solicutues a yonkes		
		public async Task<string> UpdateStatusAsync(SolicitudUpdateStatusDTO dto)
		{
			if (dto == null)
				throw new ArgumentException("Datos inválidos");

			//if (IsNullOrEmpty(dto.GuidId))
			//	throw new ArgumentException("GuidId es requerido");

			using (var transaction = await _unitofWorkSolicitudes.BeginTransactionAsync())
			{
				try
				{
					var usuarioId = _currentUserService.UserId;

					if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
					{
						throw new BusinessException(
							"No fue posible identificar al usuario.");
					}

					// =========================
					// 🔍 BUSCAR ORDEN
					// =========================
					var entity = await _unitofWorkSolicitudes.SolicitudesRepository
						.GetByGuidId(dto.GuidId);

					if (entity == null)
						throw new KeyNotFoundException("La orden no existe");

					// =========================
					// 🔥 EVITAR CAMBIO INNECESARIO
					// =========================
					if (entity.EstatusSolicitudId == (int)EstatusSolicitudEnum.Cancelada)
						throw new ArgumentException("La solicitud ya está cancelada.");

					//entity.EstatusSolicitudId = EstatusSolicitud.Cancelada;


					// =========================
					// 🔄 UPDATE
					// =========================					
					entity.EstatusSolicitudId = (int)EstatusSolicitudEnum.Cancelada;


					// =========================
					// 💾 Historial
					// =========================
					const int nuevoEstatus = (int)EstatusSolicitudEnum.Cancelada;

					entity.EstatusSolicitudId = nuevoEstatus;

					var historial = new SolicitudesHistorials
					{
						SolicitudGuidId = entity.GuidId,
						Fecha = DateTime.UtcNow,
						UsuarioId = usuarioId.Value,
						EstatusSolicitudId = nuevoEstatus,
						Comentarios = "La solicitud fue cancelada por el usuario."
					};

					await _unitofWorkSolicitudes.solicitudHistorialRepository.AddHistorialAsync(historial);


					// =========================
					// 💾 BAJA EN SOLICITUD DE YONKES    DAME ESTE CODIGO?
					// =========================
				
					var solicitudesYonkes = await _unitOfWorkSolicitudYonkes
											.SolicitudYonkeRepository
											.ObtenerPorSolicitudAsync(entity.GuidId);


					if (solicitudesYonkes != null && solicitudesYonkes.Any())
					{
						foreach (var solicitudYonke in solicitudesYonkes)
						{
							solicitudYonke.EstatusId = 6;   //Cancelada Usuario
						}
					}



					// =========================
					// 💾 GUARDAR
					// =========================
					await _unitofWorkSolicitudes.SaveChangesAsync();


					// =========================
					// ✅ COMMIT
					// =========================
					await transaction.CommitAsync();

					return entity.GuidId.ToString();
				}
				catch
				{
					// =========================
					// ❌ ROLLBACK
					// =========================
					await transaction.RollbackAsync();
					throw;
				}

			}
		}


		//Contar las solicitudes para dashboard del cliente
		public async Task<int> ContarSolicitudesUsuarioAsync()
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			return await _unitofWorkSolicitudes
				.SolicitudesRepository
				.ContarSolicitudesPorUsuarioAsync((Guid)usuarioId);
		}


		public IQueryable<Solicitud_Busqueda_DTO> VerSolicitudesByUserDashboard()
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			return _unitofWorkSolicitudes
				.SolicitudesRepository
				.GetMisSolicitudesByUserDashboard((Guid)usuarioId);
		}



		public async Task<int> ContarCotizacionesUsuarioAsync()
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			return await _unitofWorkSolicitudes
				.SolicitudesRepository
				.ContarCotizacionesPorUsuarioAsync((Guid)usuarioId);
		}

		public IQueryable<Cotizacion_List_Dashboad_DTO> GetCotizacionesByUserId()
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			return _unitofWorkSolicitudes
			.SolicitudesRepository
			.GetCotizacionesByUserId((Guid)usuarioId);
		}


		//Mas reciente del cliente

		public async Task<Solicitud_Busqueda_DTO?> ObtenerSolicitudMasRecienteAsync()
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			return await _unitofWorkSolicitudes
			.SolicitudesRepository
			.ObtenerSolicitudMasRecienteAsync(usuarioId.Value);
		}
	}
}
