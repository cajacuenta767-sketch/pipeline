using AutoMapper;
using Core.DTO.Empresas;
using Core.EntityBase;
using Core.Entitys;
using Core.Interfaces.Negocio;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Negocio
{
    public class YunkeRepository<T> : IRepositrioYunke<T> where T : BaseEntity
    {
		//injectar la BD
		private readonly AplicationDBContext _context;

		//llamar como se implementa la unificacion de las entidades
		protected readonly DbSet<T> _entities;

		private readonly IMapper _mapper;

		public YunkeRepository(AplicationDBContext context,
								IMapper mapper)
		{
			_context = context;
			_mapper = mapper;

			//indicando que puede toamr el tipo de operacion en base a BaseEntity (Post, GEt, Etc.)
			_entities = context.Set<T>();
		}


		//Yonkes
		public async Task<IQueryable<YonkesListDTO>> getYonkesByCiudad(int? ciudadId = null)
		{
			var query = _context.Yonkes
			   .AsNoTracking()
			   .AsQueryable();

			// Si se recibió ciudadId, filtrar por ciudad
			if (ciudadId.HasValue && ciudadId.Value > 0)
			{
				query = query.Where(x => x.CiudadId == ciudadId.Value);
			}

			return query
			   .Select(prod => new YonkesListDTO
			   {
				   Id = prod.Id,
				   GuidId = prod.GuidId,
				   Estatus = prod.Estatus,
				   Nombre = prod.Nombre,
				   Responsable = prod.Responsable,
				   LogoUrl = prod.LogoUrl,
				   Telefono = prod.Telefono,
				   Correo = prod.Correo,
				   Direccion = prod.Direccion,
				   CP = prod.CP,
				   CiudadId = prod.CiudadId,
				   Ciudad = prod.Ciudades.Ciudad,
				   Latitud = prod.Latitud,
				   Longitud = prod.Longitud,

				  

			   }).OrderBy(x => x.Nombre);
		}

		public async Task<YonkesListDTO?> getYunkeGuidById(Guid guidId)
		{
			return await _context.Yonkes
			  .Where(x => x.GuidId == guidId)
			  .Select(prod => new YonkesListDTO
			  {
				  Id = prod.Id,
				  CreateBy = (Guid)prod.CreateBy,
				  Autorizado = prod.Autorizado,
				  GuidId = prod.GuidId,
				  Estatus = prod.Estatus,
				  Nombre = prod.Nombre,
				  Responsable = prod.Responsable,
				  LogoUrl = prod.LogoUrl,
				  Telefono = prod.Telefono,
				  Correo = prod.Correo,
				  Direccion = prod.Direccion,
				  CP = prod.CP,
				  CiudadId = prod.CiudadId,
				  Ciudad = prod.Ciudades.Ciudad,

				  Latitud = prod.Latitud,
				  Longitud	= prod.Longitud,

				  //Coberturas
				  Coberturas = _context.YonkesCoberturas
					.Where(c => c.YonkeGuidId == prod.GuidId && c.Activo)
					.Select(c => new YonkeCoberturasDTO
					{				
						GuidId = c.GuidId,
						YonkeGuidId = c.YonkeGuidId,					
						Ciudad = c.Ciudades.Ciudad,					
					}).ToList()


			  }).FirstOrDefaultAsync();	
		}

		public async Task<Yonkes?> getYunkeById(int id)
		{
			return await _context.Yonkes.Where(x=> x.Id == id).FirstOrDefaultAsync();
		}

		public async Task addYunke(Yonkes YonkesaveDTO, CancellationToken cancellationToken = default)
		{
			await _context.AddAsync(YonkesaveDTO, cancellationToken);
		}


		public void UpdateYunke(Yonkes yunkeGrabarDTO)
		{
			yunkeGrabarDTO.CreateAt = DateTime.Now;
			yunkeGrabarDTO.Estatus = true;

			_context.Update(yunkeGrabarDTO);
		}

		public void BajaYunke(Yonkes yunkeBaja)
		{
			_context.Update(yunkeBaja);
		}

		public async Task<List<string>> ObtenerTokensFirebaseAsync(List<Guid> YonkeGuidId)
		{
			if (YonkeGuidId == null || !YonkeGuidId.Any())
			{
				return new List<string>();
			}

			return await (
				from dispositivo in _context.YonkesDispositivos
				join yonke in _context.Yonkes.AsNoTracking()
					on dispositivo.YonkeGuidId equals yonke.GuidId
				where yonke.Estatus
					&& dispositivo.Activo
					&& !string.IsNullOrWhiteSpace(dispositivo.FirebaseToken)
					&& YonkeGuidId.Contains(yonke.GuidId)
				select dispositivo.FirebaseToken
			)
			.Distinct()
			.ToListAsync();
		}

		public async Task<bool> ExisteAsync(string nombre, string correo, string telefono)
		{
			return await _context.Yonkes
			 .AsNoTracking()
			 .AnyAsync(x =>
				 x.Nombre == nombre &&
				 x.Correo == correo &&
				 x.Telefono == telefono);
		}

		public async Task<Yonkes?> getGuidById(Guid guidId)
		{
			return await _context.Yonkes.Where(x => x.GuidId == guidId).FirstOrDefaultAsync();
		}

		//obtener por correo usado en login
		public async Task<Yonkes> ObtenerPorCorreoAsync(string correo)
		{
			return await _context.Yonkes
				.FirstOrDefaultAsync(x => x.Correo == correo && x.Estatus == true);
		}
	}
}
