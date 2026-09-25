using Core.Entitys;

namespace Core.Interfaces.Login_Cliente
{
	public interface ILoginRepositorio
	{
		

		Task<Clientes?> ObtenerPorGoogleIdAsync(string googleId);

		Task<Clientes?> ObtenerPorAppleIdAsync(string appleId);

		Task<Clientes?> ObtenerPorCorreoAsync(string correo);   //yaregsitrado en mi sql

		Task AgregarAsync(Clientes cliente);


		//Otp Sms agregar nuevos clientes

		Task<Clientes?> ObtenerPorTelefonoAsync(string telefono);

		Task<Clientes?> ObtenerPorGuidAsync(Guid guidId);




	}
}
