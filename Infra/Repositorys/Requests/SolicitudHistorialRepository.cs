using AutoMapper;
using Core.EntityBase;
using Core.Interfaces.Requests.historials;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Requests
{
	public class SolicitudHistorialRepository<T> : ISolicitudHistorialRepository<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		public SolicitudHistorialRepository(AplicationDBContext context,
											IMapper mapper)
		{
			_context = context;
			_mapper = mapper;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			_entities = context.Set<T>();

		}

		public async Task AddHistorialAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);	
		}
	}
}
