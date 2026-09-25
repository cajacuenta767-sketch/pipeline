using Core.Entitys;
using Core.Interfaces.Login_Cliente;
using Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositorys.Login
{
	public class LoginClienteRepository : ILoginRepositorio
	{
		//injectar la BD
		private readonly AplicationDBContext _context;

		public LoginClienteRepository(AplicationDBContext context)
		{
			_context = context;
		}

		//Google

		public async Task<Clientes?> ObtenerPorGoogleIdAsync(string googleId)
		{
			return await _context.Clientes
				.FirstOrDefaultAsync(x =>
					x.GoogleId == googleId &&
					x.Activo);
		}


		//Apple
		public async Task<Clientes?> ObtenerPorAppleIdAsync(string appleId)
		{
			return await _context.Clientes
				.FirstOrDefaultAsync(x =>
					x.AppleId == appleId &&
					x.Activo);
		}


		//Correo Yonke
		public async Task<Clientes?> ObtenerPorCorreoAsync(string correo)
		{
			return await _context.Clientes.FirstOrDefaultAsync(x => x.Correo == correo);
		}


		public async Task AgregarAsync(Clientes cliente)
		{
			await _context.Clientes.AddAsync(cliente);
		}


		//Telefono Cliente
		public async Task<Clientes?> ObtenerPorTelefonoAsync(string telefono)
		{
			if (string.IsNullOrWhiteSpace(telefono))
				return null;

			telefono = telefono.Trim();

			return await _context.Clientes
				.AsNoTracking()
				.FirstOrDefaultAsync(x =>
					x.Telefono == telefono &&
					x.Activo);
		}



		public async Task<Clientes?> ObtenerPorGuidAsync(Guid guidId)
		{
			if (guidId == Guid.Empty)
				return null;

			return await _context.Clientes
				.FirstOrDefaultAsync(x =>
					x.GuidId == guidId &&
					x.Activo);
		}



	}
}
