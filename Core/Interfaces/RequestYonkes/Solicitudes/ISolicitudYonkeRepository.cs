namespace Core.Interfaces.RequestYonkes.Solicitudes
{
	public interface ISolicitudYonkeRepository
	{
		Task EnviarSolicitudAsync(Guid solicitudGuidId);
	}
}
