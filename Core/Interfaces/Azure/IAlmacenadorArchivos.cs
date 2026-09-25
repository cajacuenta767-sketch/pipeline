namespace Core.Interfaces.Azure
{
	public interface IAlmacenadorArchivos
	{
		Task<string> GuardarArchivo(byte[] contenido,
		  string extension,
		  string contenedor,
		  string rutaBlob,
		  string contentType);

		Task<string> EditarArchivo(byte[] contenido,
			 string extension,
			 string contenedor,
			 string ruta,
			 string contentType);

		Task BorrarArchivo(string ruta,
			 string contenedor);
	}
}
