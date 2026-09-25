using AutoMapper;
using Core.DTO.Solicitudes.Estatus;
using Core.EntityBase;
using Core.Interfaces.Requests;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Requests
{
	public class SolicitudesEstatusRepository<T> : ISolicitudesEstatusRepository<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		public SolicitudesEstatusRepository(AplicationDBContext context, IMapper mapper)
		{
			_context = context;	
			_mapper = mapper;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			_entities = context.Set<T>();
		}

		public async Task<IList<Solicitud_Estatus_List_DTO>> GetSolicitudEstatusAsync()
		{
			return await _context.SolicitudesEstatus
				.AsNoTracking()
				.Select(x=> new Solicitud_Estatus_List_DTO
				{
					Id = x.Id,
					Estatus = x.Estatus,
				}).OrderBy(x=> x.Id).ToListAsync();
		}
	}
}
