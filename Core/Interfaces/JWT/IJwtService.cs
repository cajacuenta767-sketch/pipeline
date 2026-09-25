using Core.Entitys;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces.JWT
{
	public interface IJwtService
	{
		string GenerarTokenCliente(Clientes cliente);

		string GenerarTokenYunke(Yonkes yonke,	IdentityUser user,	IEnumerable<string> roles);
	}
}
