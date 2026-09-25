using Core.EntityBase;

namespace Core.Entitys
{
	public class SolicitudesCiudades : BaseEntity
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; } = Guid.NewGuid();


		// FK hacia Solicitudes.GuidId
		public Guid SolicitudGuidId { get; set; }


		// FK hacia Ciudades.Id
		public int CiudadId { get; set; }



		public virtual Solicitudes Solicitudes { get; set; }

		public virtual Ciudades Ciudades { get; set; }
	}
}
