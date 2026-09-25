using Core.EntityBase;

namespace Core.Entitys
{
	public class SolicitudYonkesEstatus : BaseEntity
	{
		public SolicitudYonkesEstatus()
		{
			solicitudYonkes = new HashSet<SolicitudYonkes>();
		}


		public int Id { get; set; }
		public string EstatusSolicitud { get; set; }


		public ICollection<SolicitudYonkes> solicitudYonkes { get; set; }

		
	}
}
