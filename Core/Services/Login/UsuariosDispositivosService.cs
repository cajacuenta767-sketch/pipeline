using Core.DTO.Login.usuarioDispo;
using Core.Entitys;
using Core.Interfaces.Login.UserDispotivos;
using Core.Interfaces.Login_Cliente;

namespace Core.Services.Login
{
	public class UsuariosDispositivosService : IUsuariosDispositivosService
	{
		private readonly IUnitOfWorkCliente _unitOfWork;

		public UsuariosDispositivosService(IUnitOfWorkCliente unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<bool> RegistrarDispositivoAsync(string usuarioId, RegistrarDispositivoRequest request, CancellationToken cancellationToken = default)
		{
			var dispositivo = await _unitOfWork.UsuariosDispositivosRepository
				.ObtenerPorTokenAsync(request.FirebaseToken, cancellationToken);

			if (dispositivo == null)
			{
				dispositivo = new UsuariosDispositivos
				{
					UsuarioId = usuarioId,
					FirebaseToken = request.FirebaseToken,
					Plataforma = request.Plataforma,
					Modelo = request.Modelo,
					Activo = true,
					FechaRegistro = DateTime.UtcNow,
					UltimoAcceso = DateTime.UtcNow
				};

				await _unitOfWork.UsuariosDispositivosRepository
					.AgregarAsync(dispositivo, cancellationToken);
			}
			else
			{
				dispositivo.UsuarioId = usuarioId;
				dispositivo.Plataforma = request.Plataforma;
				dispositivo.Modelo = request.Modelo;
				dispositivo.Activo = true;
				dispositivo.UltimoAcceso = DateTime.UtcNow;

				_unitOfWork.UsuariosDispositivosRepository
					.Actualizar(dispositivo);
			}

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return true;
		}

		public async Task<List<string>> ObtenerTokensUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default)
		{
			var dispositivos = await _unitOfWork.UsuariosDispositivosRepository
				.ObtenerTokensUsuarioAsync(usuarioId, cancellationToken);

			return dispositivos
				.Select(x => x.FirebaseToken)
				.Distinct()
				.ToList();
		}
	}
}
