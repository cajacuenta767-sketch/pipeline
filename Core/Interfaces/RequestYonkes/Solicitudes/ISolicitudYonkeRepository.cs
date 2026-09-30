using Core.Entitys;

namespace Core.Interfaces.RequestYonkes.Solicitudes
{
	public interface ISolicitudYonkeRepository
	{
		Task EnviarSolicitudAsync(Guid solicitudGuidId);

		//Solicitud mas reciente de cada yonke logeado
		Task<SolicitudYonkes?> SolicitudRecienteByYonke(Guid yonkeGuidId, CancellationToken cancellation);
	}
}
