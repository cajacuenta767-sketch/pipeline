using AutoMapper;
using Core.DTO.Solicitudes.Requests;
using Core.DTO.SolocitudCotizaciones;
using Core.EntityBase;
using Core.Entitys;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Requests;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;

namespace Infra.Repositorys.Requests
{
	public class SolicitudesRepository<T> : ISolicitudesRepository<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		private readonly ICurrentUserService _currentUserService;
		public SolicitudesRepository(AplicationDBContext context,
									 IMapper mapper,
									 ICurrentUserService currentUserService)
		{
			_context = context;
			_mapper = mapper;
			_currentUserService = currentUserService;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			//_entities = context.Set<T>();
		}

		
		public IQueryable<Solicitud_Busqueda_DTO> GetSolicitudesByUserId(DateTime desde, DateTime hasta, Guid userId)
		{
			//Validacion del usuario enviado contra el de login
			var usuarioId = _currentUserService.UserId;

			if (usuarioId.Value != userId)
			{
				throw new BusinessException(
					"El usuario enviado no corresponde al usuario autenticado.");
			}

			//Pendiente, Proceso, Cotizada, Aceptada y Cerrada 
			var estatusPermitidos = new List<int>
			{
				1, 2, 3, 4, 7
			};

			return _context.Solicitudes
				.AsNoTracking()
				.Where(x => x.FechaCreacion >= desde.Date &&
							x.FechaCreacion <= hasta.Date &&
							x.UsuarioId == userId &&
							estatusPermitidos.Contains(x.EstatusSolicitudId))
				.Select(x => new Solicitud_Busqueda_DTO
				{
					Id = x.Id,
					FechaCreacion = x.FechaCreacion,
					GuidId = x.GuidId,
					EstatusSolicitudId = x.EstatusSolicitudId,
					EstatusSolicitud = x.SolicitudEstatus.Estatus,
					UsuarioId = x.UsuarioId,
					MarcaId = x.MarcaId,
					Marca = x.Marcas.Marca,
					ModeloId = x.ModeloId,
					Modelo = x.Modelos.Modelo,
					Año = x.Año,
					Motor = x.Motor,
					Transmicion = x.Transmicion,
					PiezaBuscada = x.PiezaBuscada,
					NumeroParte = x.NumeroParte,
					Descripcion = x.Descripcion,

					TotalCotizaciones = _context.SolicitudCotizaciones
					.Count(c =>
						c.Activo &&
						c.SolicitudYonkes.Solicitudes.GuidId == x.GuidId),

					Folio = x.Folio,
					FechaCierre = x.FechaCierre,
					Cerrada = x.Cerrada
				});
		}

		public async Task<Solicitud_Busqueda_DTO> GetSolicitudByGuidId(Guid guidId )
		{	
			return await _context.Solicitudes
				.AsNoTracking()
				.Where(x => x.GuidId == guidId)
				.Select(x => new Solicitud_Busqueda_DTO
				{
					Id = x.Id,
					FechaCreacion = x.FechaCreacion,
					GuidId = x.GuidId,
					EstatusSolicitudId = x.EstatusSolicitudId,
					EstatusSolicitud = x.SolicitudEstatus.Estatus,
					UsuarioId = x.UsuarioId,
					MarcaId = x.MarcaId,
					Marca = x.Marcas.Marca,
					ModeloId = x.ModeloId,
					Modelo = x.Modelos.Modelo,
					Año = x.Año,
					Motor = x.Motor,
					Transmicion = x.Transmicion,
					PiezaBuscada = x.PiezaBuscada,
					NumeroParte = x.NumeroParte,
					Descripcion = x.Descripcion,

					TotalCotizaciones = _context.SolicitudCotizaciones
					.Count(c =>
						c.Activo &&
						c.SolicitudYonkes.Solicitudes.GuidId == x.GuidId),

					Folio = x.Folio,
					FechaCierre = x.FechaCierre,
					Cerrada = x.Cerrada
				}).FirstOrDefaultAsync();
			
		}

		
		public async Task AddAsync(T entity, CancellationToken cancellationToken)
		{
			await _context.Set<T>().AddAsync(entity, cancellationToken);
		}


		public void UpdateSolicitud(Solicitudes solicitudUpdateDTO)
		{
			_context.Update(solicitudUpdateDTO);
		}

		public void BajaSolicitud(Guid guidId)
		{
			_context.Update(guidId);
		}

	

		public async Task<Solicitudes?> GetByGuidId(Guid guidId)
		{
			return await _context.Solicitudes
						 .FirstOrDefaultAsync(x => x.GuidId == guidId);
		}

		public async Task<string> GenerarFolioSolicitudAsync()
		{
			int anio = DateTime.Now.Year;

			var folio = await _context.Folios
				.FirstOrDefaultAsync(x => x.Tipo == "SOL" && x.Anio == anio);

			if (folio == null)
			{
				folio = new Folios
				{
					Tipo = "SOL",
					Anio = anio,
					Consecutivo = 1
				};

				_context.Folios.Add(folio);
			}
			else
			{
				folio.Consecutivo++;
			}

			await _context.SaveChangesAsync();

			return $"SOL-{folio.Consecutivo:D5}/{anio}";
		}



		/// <summary>
		/// Conteo de las solicitues by userId
		/// </summary>
		/// <param name="usuarioId"></param>
		/// <returns></returns>
		public async Task<int> ContarSolicitudesPorUsuarioAsync(Guid usuarioId)
		{
			int[] estatusPermitidos =
			{
				(int)EstatusSolicitudEnum.Pendiente,
				(int)EstatusSolicitudEnum.Enviada,
				(int)EstatusSolicitudEnum.Cotizada
			};

			return await _context.Solicitudes
				.CountAsync(x =>
					x.UsuarioId == usuarioId &&
					estatusPermitidos.Contains(x.EstatusSolicitudId));
		}

		//Ver todas las solicitudes
		public IQueryable<Solicitud_Busqueda_DTO> GetMisSolicitudesByUserDashboard(Guid userId)
		{
			//Pendiente, Proceso, Cotizada, Aceptada y Cerrada 
			var estatusPermitidos = new List<int>
			{
				1, 2, 3, 4
			};

			return _context.Solicitudes
				.AsNoTracking()
				.Where(x => x.UsuarioId == userId &&
							estatusPermitidos.Contains(x.EstatusSolicitudId))
				.Select(x => new Solicitud_Busqueda_DTO
				{
					Id = x.Id,
					FechaCreacion = x.FechaCreacion,
					GuidId = x.GuidId,
					EstatusSolicitudId = x.EstatusSolicitudId,
					EstatusSolicitud = x.SolicitudEstatus.Estatus,
					UsuarioId = x.UsuarioId,
					MarcaId = x.MarcaId,
					Marca = x.Marcas.Marca,
					ModeloId = x.ModeloId,
					Modelo = x.Modelos.Modelo,
					Año = x.Año,
					Motor = x.Motor,
					Transmicion = x.Transmicion,
					PiezaBuscada = x.PiezaBuscada,
					NumeroParte = x.NumeroParte,
					Descripcion = x.Descripcion,

					TotalCotizaciones = _context.SolicitudCotizaciones
					.Count(c =>
						c.Activo &&
						c.SolicitudYonkes.Solicitudes.GuidId == x.GuidId),

					Folio = x.Folio,
					FechaCierre = x.FechaCierre,
					Cerrada = x.Cerrada
				});
		}



		/// <summary>
		/// Contar las cotizaciones que tiene por usuario
		/// </summary>
		/// <param name="usuarioId"></param>
		/// <returns></returns>
		public async Task<int> ContarCotizacionesPorUsuarioAsync(Guid usuarioId)
		{
			return await _context.SolicitudCotizaciones
				.CountAsync(c =>
					c.Activo &&
					c.SolicitudYonkes
						.Solicitudes
						.UsuarioId == usuarioId);
		}

		//Listado de cotizaciones activas por cada usuario 
		public IQueryable<Cotizacion_List_Dashboad_DTO> GetCotizacionesByUserId(Guid userId)
		{
			return _context.SolicitudCotizaciones
				.AsNoTracking()
				.Where(c =>
					c.Activo &&
					c.SolicitudYonkes.Solicitudes.UsuarioId == userId)
				.Select(c => new Cotizacion_List_Dashboad_DTO
				{
					Id = c.Id,
					GuidId = c.GuidId,
					SolicitudYonkeGuidId = c.SolicitudYonkeGuidId,
					FechaCreacionCotizacion = c.FechaCreacion,
			
					Precio = c.Precio,
					Disponible = c.Disponible,
					EsNueva = c.EsNueva,		
					Comentarios = c.Comentarios,
					TieneGarantia = c.TieneGarantia,
					DiasGarantia = c.DiasGarantia,
					EnvioDisponible = c.EnvioDisponible,
					CostoEnvio = c.CostoEnvio,			
					EstatusId = c.EstatusId,			

					// Datos de la solicitud			
					Folio = c.SolicitudYonkes.Solicitudes.Folio,
					PiezaBuscada = c.SolicitudYonkes.Solicitudes.PiezaBuscada,
					Marca = c.SolicitudYonkes.Solicitudes.Marcas.Marca,
					NumeroParte = c.NumeroParte,

					// Usuario dueño de la solicitud
					UsuarioId = c.SolicitudYonkes.Solicitudes.UsuarioId
				});
		}




		//Soliciud mas reciente del cliente
		public async Task<Solicitud_Busqueda_DTO?> ObtenerSolicitudMasRecienteAsync(Guid userId)
		{
			var estatusPermitidos = new List<int>
			{
				1, 2, 3, 4
			};

			return await _context.Solicitudes
			.AsNoTracking()
			.Where(x => x.UsuarioId == userId)
			.OrderByDescending(x => x.FechaCreacion)
			.Select(x => new Solicitud_Busqueda_DTO
			{
				Id = x.Id,
				GuidId = x.GuidId,
				FechaCreacion = x.FechaCreacion,
				EstatusSolicitudId = x.EstatusSolicitudId,
				EstatusSolicitud = x.SolicitudEstatus.Estatus,
				UsuarioId = x.UsuarioId,
				MarcaId = x.MarcaId,
				Marca = x.Marcas.Marca,
				ModeloId = x.ModeloId,
				Modelo = x.Modelos.Modelo,
				Año = x.Año,
				Motor = x.Motor,
				Transmicion = x.Transmicion,
				PiezaBuscada = x.PiezaBuscada,
				NumeroParte = x.NumeroParte,
				Descripcion = x.Descripcion,
				Folio = x.Folio,
				FechaCierre = x.FechaCierre,
				Cerrada = x.Cerrada,

				TotalCotizaciones = _context.SolicitudCotizaciones
					.Count(c =>
						c.Activo &&
						c.SolicitudYonkes.Solicitudes.GuidId == x.GuidId)
			}).FirstOrDefaultAsync();
		}



		public async Task<int> CountAsync(Expression<Func<Solicitudes, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _context.Solicitudes
				.CountAsync(predicate, cancellationToken);
		}




		

	


	}
}
