namespace ApiYonke.Responses
{
	public class ApiResponse<T>
	{
		public ApiResponse(T data)
		{
			//setear 
			Data = data;
		}
		//devuielve las mismas respuestas para todas las API
		public T Data { get; set; }
	}
}
