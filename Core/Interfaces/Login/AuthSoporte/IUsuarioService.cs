using Core.DTO.Login.AuthSoporte;

namespace Core.Interfaces.Login.AuthSoporte
{
	public interface IUsuarioService
	{
		Task<string> CrearUsuarioSoporteAsync(
			CrearUsuarioSoporteDTO request,
			CancellationToken cancellationToken = default);

		Task<LoginSoporteResponseDTO> LoginAsync(
		   LoginSoporteDTO request,
		   CancellationToken cancellationToken = default);
	}
}
