using AutoMapper;
using Core.DTO.Solicitudes.imagnees;
using Core.EntityBase;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Azure;
using Core.Interfaces.Requests;
using Infra.DataContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.Repositorys.Requests
{
	public class SolicitudesImagenesRepository<T> : ISolicitudImagenesRepository<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		private readonly IAlmacenadorArchivos _almacenadorArchivos;

		private readonly IUnitOfWorkSolicitudes _unitOfWorkSolicitudes;

		//nombre de la carpeta donde se graban los datos en Azure
		private readonly string contenedor = "solicitudes";

		public SolicitudesImagenesRepository(AplicationDBContext context,
										     IMapper mapper,
											 IAlmacenadorArchivos almacenadorArchivos,
											 IUnitOfWorkSolicitudes unitOfWorkSolicitudes)
		{
			_context = context;
			_mapper = mapper;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			_entities = context.Set<T>();

			_almacenadorArchivos = almacenadorArchivos;	
			_unitOfWorkSolicitudes = unitOfWorkSolicitudes;
		}

		public SolicitudesImagenesRepository(AplicationDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task AgregarImagenAsync(SolicitudesImagenes entity)
		{
			await _context.SolicitudesImagenes.AddAsync(entity);
		}

		public async Task<IList<SolicitudesImagenes_List_DTO>> GetImagenesBySolicitudAsync(Guid solicitudGuidId)
		{
			return await _context.SolicitudesImagenes
				.AsNoTracking()
				.Where(x => x.Solicitudes.GuidId == solicitudGuidId)
				.Select(x => new SolicitudesImagenes_List_DTO
				{
					GuidId = x.GuidId,
					UrlImagen = x.UrlImagen,
					FechaCreacion = x.FechaCreacion
				}).ToListAsync();
		}

		public async Task<SolicitudesImagenes_DTO?> GetImagenByGuidIdAsync(Guid imagenGuidId)
		{
			return await _context.SolicitudesImagenes
			  .AsNoTracking()
			  .Where(x => x.GuidId == imagenGuidId)
			  .Select(x => new SolicitudesImagenes_DTO
			  {
				  GuidId = x.GuidId,
				  UrlImagen = x.UrlImagen,
				  FechaCreacion = x.FechaCreacion,
				  SolicitudGuidId = x.GuidId
			  }) .FirstOrDefaultAsync();
		}

		public async Task<bool> EliminarImagenAsync(Guid imagenGuidId)
		{
			var imagen = await _context.SolicitudesImagenes
				.FirstOrDefaultAsync(x => x.GuidId == imagenGuidId);


			if (imagen == null)
				return false;


			_context.SolicitudesImagenes.Remove(imagen);


			return true;
		}

		public async Task<SolicitudesImagenes?> GetByGuidId(Guid guidId)
		{
			return await _context.SolicitudesImagenes.FirstOrDefaultAsync(x => x.GuidId == guidId);
		}

	
	}
}
