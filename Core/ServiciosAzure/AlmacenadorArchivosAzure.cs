using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Interfaces.Azure;
using Microsoft.Extensions.Configuration;

namespace Core.ServiciosAzure
{
	public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
	{
		//conexion de azure storage 
		private readonly string connectionString;

		//instanciar la conexion
		public AlmacenadorArchivosAzure(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("AzureStorage");
		}

		//borrar archivo
		public async Task BorrarArchivo(string ruta, string contenedor)
		{
			if (string.IsNullOrEmpty(ruta))
				return;


			var cliente = new BlobContainerClient(
				connectionString,
				contenedor);


			var blob = cliente.GetBlobClient(ruta);


			await blob.DeleteIfExistsAsync();
		}

		//update archivo
		public async Task<string> EditarArchivo(byte[] contenido,
			string extension, string contenedor, string ruta,
			string contentType)
		{
			await BorrarArchivo(ruta, contenedor);
			return await GuardarArchivo(contenido, extension, contenedor, ruta, contentType);
		}

		#region Guardar no usado
		//grabar archivo
		//public async Task<string> GuardarArchivo(byte[] contenido, string extension,
		//	string contenedor, string rutaBlob, string contentType)
		//{
		//	//si no hay contenedor lo crea
		//	var cliente = new BlobContainerClient(connectionString, contenedor);

		//	await cliente.CreateIfNotExistsAsync();

		//	cliente.SetAccessPolicy(PublicAccessType.Blob);

		//	//genera el nombre de archivo aleatorio
		//	var archivoNombre = $"{Guid.NewGuid()}{extension}";
		//	var blob = cliente.GetBlobClient(archivoNombre);

		//	//pasar el archivo
		//	var blobUploadOptions = new BlobUploadOptions();
		//	var blobHttpHeader = new BlobHttpHeaders();
		//	blobHttpHeader.ContentType = contentType;
		//	blobUploadOptions.HttpHeaders = blobHttpHeader;

		//	//graba el archivo en azure
		//	await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);

		//	//retorna la url para grabar en la tabla
		//	return blob.Uri.ToString();
		//}
		#endregion

		public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor,	string rutaBlob, string contentType)
		{
			// si no existe el contenedor lo crea
			var cliente = new BlobContainerClient(connectionString,	contenedor);

			await cliente.CreateIfNotExistsAsync();


			cliente.SetAccessPolicy(PublicAccessType.Blob);


			// Usa la ruta que viene del servicio
			var blob = cliente.GetBlobClient(rutaBlob);


			var blobUploadOptions = new BlobUploadOptions
			{
				HttpHeaders = new BlobHttpHeaders
				{
					ContentType = contentType
				}
			};


			await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);


			return blob.Uri.ToString();
		}
	}
}
