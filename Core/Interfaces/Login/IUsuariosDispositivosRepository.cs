using Core.Entitys;

namespace Core.Interfaces.Login
{
	public interface IUsuariosDispositivosRepository
	{
		Task<UsuariosDispositivos?> ObtenerPorTokenAsync(string firebaseToken, CancellationToken cancellationToken = default); 
		Task<List<UsuariosDispositivos>> ObtenerTokensUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default);
		Task AgregarAsync(UsuariosDispositivos dispositivo, CancellationToken cancellationToken = default);
		void Actualizar(UsuariosDispositivos dispositivo);
	}
}
