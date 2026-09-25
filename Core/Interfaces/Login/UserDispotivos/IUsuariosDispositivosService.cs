using Core.DTO.Login.usuarioDispo;

namespace Core.Interfaces.Login.UserDispotivos
{
	public interface IUsuariosDispositivosService
	{
		Task<bool> RegistrarDispositivoAsync(
			string usuarioId,
			RegistrarDispositivoRequest request,
			CancellationToken cancellationToken = default);

		Task<List<string>> ObtenerTokensUsuarioAsync(
			string usuarioId,
			CancellationToken cancellationToken = default);
	}
}
