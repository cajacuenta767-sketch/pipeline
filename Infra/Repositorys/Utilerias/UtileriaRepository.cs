using AutoMapper;
using Core.DTO.Utilerias.Ciudades;
using Core.DTO.Utilerias.Marcas;
using Core.DTO.Utilerias.Models;
using Core.DTO.Utilerias.States;
using Core.EntityBase;
using Core.Entitys;
using Core.Interfaces.Utilerias;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Utilerias
{
	public class UtileriaRepository<T> : IRepositorioUtilerias<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		public UtileriaRepository(AplicationDBContext context,
								IMapper mapper)
		{
			_context = context;
			_mapper = mapper;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			_entities = context.Set<T>();
		}


		#region States

		public async Task<IList<EntidadesListDTO>> getEntidadesList()
		{
			return await _context.Entidades
			.AsNoTracking()
			.Select(x => new EntidadesListDTO
			{
				Id = x.Id,
				Entidad = x.Entidad,
			}).ToListAsync();
		}

		public async Task<Entidades> getEntidadById(int id)
		{
			return await _context.Entidades
				.AsNoTracking()
				.Where(x => x.Id == id).FirstOrDefaultAsync();
		}

		#endregion

		#region Citys
		public async Task<IList<CiudadesListDTO>> getCiudadesList(int entidadId)
		{
			return await _context.Ciudades.Where(x => x.EntidadId == entidadId)
				.AsNoTracking()
				.Select(x => new CiudadesListDTO
				{
					Id = x.Id,
					Ciudad = x.Ciudad,
					Entidade = x.Entidades.Entidad,
					EntidadId = x.EntidadId					
				}).OrderBy(x=> x.Ciudad).ToListAsync();
		}

		public async Task<IList<int>> getListadoCiudadesId()
		{
			return await _context.Ciudades
				.AsNoTracking()
				.OrderBy(x => x.Id)
				.Select(x => x.Id)
				.ToListAsync();
		}

		public async Task<Ciudades> getCiudadById(int id)
		{
			return await _context.Ciudades
				.AsNoTracking()
				.Where(x => x.Id == id).FirstOrDefaultAsync();
		}


		#endregion



		#region Marcas
		public async Task<IList<MarcasListDTO>> getMarcas()
		{
			return await _context.Marcas
				.AsNoTracking()
				.Select(x=> new MarcasListDTO
				{
					Id = x.Id,
					Marca = x.Marca,
				}).OrderBy(x=> x.Marca).ToListAsync();
		}

		public async Task<Marcas> getMarcaById(int id)
		{
			return await _context.Marcas
				.AsNoTracking()
				.Where(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task addMarca(Marcas marcaSave)
		{
			await _context.AddAsync(marcaSave);
		}

		public void updataMarca(Marcas marcaUpdate)
		{
			_context.AddAsync(marcaUpdate);
		}

		public async Task<bool> ExisteMarca(int id)
		{
			return await _context.Marcas.AnyAsync(x => x.Id == id);
		}





		#endregion



		#region Modelos
		public async Task<IList<ModelosListDTO>> getModelos(int marcaId)
		{
			return await _context.Modelos.Where(x => x.MarcaId == marcaId)
				.AsNoTracking()
				.Select(x => new ModelosListDTO
				{
					Id = x.Id,
					Modelo = x.Modelo,
					MarcaId = x.MarcaId,
					Marca = x.Marcas.Marca
				}).OrderBy(x => x.Modelo).ToListAsync();
		}

		public async Task<Modelos> getModeloById(int id)
		{
			return await _context.Modelos
				.AsNoTracking()
				.Where(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task addModelo(Modelos modeloSave)
		{
			await _context.AddAsync(modeloSave);
		}

		public void updataModelo(Modelos modeloUpdate)
		{
			_context.AddAsync(modeloUpdate);
		}

		public async Task<bool> ExisteModelo(int id)
		{
			return await _context.Modelos.AnyAsync(x => x.Id == id);
		}

	
		#endregion

	}
}
