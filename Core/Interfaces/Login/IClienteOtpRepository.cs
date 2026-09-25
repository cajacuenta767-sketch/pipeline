namespace Core.Interfaces.Login
{
	public interface IClienteOtpRepository
	{
		Task<Entitys.ClienteOtps?> ObtenerOtpActivoAsync(
		string telefono,
		CancellationToken cancellationToken = default);

		Task AgregarAsync(
			Entitys.ClienteOtps otp,
			CancellationToken cancellationToken = default);

		Task DesactivarOtpsAnterioresAsync(
			string telefono,
			CancellationToken cancellationToken = default);

		Task ActualizarAsync(
			Entitys.ClienteOtps otp,
			CancellationToken cancellationToken = default);

		Task<Entitys.ClienteOtps?> ObtenerBloqueoTelefonoAsync(string telefono, CancellationToken cancellationToken = default);
	}
}
