using AutoMapper;
using Core.EntityBase;
using Core.Entitys;
using Core.Interfaces.Auth;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Soporte.Empresa
{
	public class SoporteRepository<T> : ISoporteRepository<T> where T : BaseEntity
	{
		//injectar la BD
		private readonly AplicationDBContext _context;
		private readonly IMapper _mapper;


		//nombre de la carpeta donde se graban los datos en Azure
		private readonly string contenedor = "logos";
		public SoporteRepository(AplicationDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		#region Yonkes

		public async Task<IEnumerable<Yonkes>> GetAllEmpresasAsync()
		{
			return (IEnumerable<Yonkes>)await _context.Yonkes.ToListAsync();
		}
		public async Task<Yonkes> GetEmpresaById(int id)
		{
			return await _context.Yonkes.Where(x => x.Id == id).FirstOrDefaultAsync();
		}


		public async Task AddEmpresa(Yonkes empresaGrabarDTO)
		{
			await _context.AddAsync(empresaGrabarDTO);
		}

		public void UpdateEmpresa(Yonkes empresaGrabarDTO)
		{
			_context.Update(empresaGrabarDTO);
		}

		#endregion


		#region Acceso Detalles
		public async Task<IEnumerable<AccesoDetalles>> GetAllAccesoDetallesAsync()
		{
			return (IEnumerable<AccesoDetalles>)await _context.AccesoDetalles.OrderBy(x => x.Yonkes.Nombre).ToListAsync();
		}

		public async Task<AccesoDetalles> GetAccesoDetallesById(int id)
		{
			return await _context.AccesoDetalles.Where(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task AddAccesoDetalle(AccesoDetalles accesoDetalles)
		{
			await _context.AddAsync(accesoDetalles);
		}

		public void UpdateAccesoDetalle(AccesoDetalles accesoDetalles)
		{
			_context.Update(accesoDetalles);
		}
		#endregion


	}
}
