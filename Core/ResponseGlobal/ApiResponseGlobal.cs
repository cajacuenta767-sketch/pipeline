namespace Core.ResponseGlobal
{
	public class ApiResponseGlobal<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }
		public int StatusCode { get; set; }
		public object Errors { get; set; }

		public static ApiResponseGlobal<T> Ok(T data, string message = "OK")
		{
			return new ApiResponseGlobal<T>
			{
				Success = true,
				Message = message,
				Data = data,
				StatusCode = 200,
				Errors = null
			};
		}

		public static ApiResponseGlobal<T> Fail(string message, int statusCode = 400, object errors = null)
		{
			return new ApiResponseGlobal<T>
			{
				Success = false,
				Message = message,
				Data = default,
				StatusCode = statusCode,
				Errors = errors
			};
		}

	}
}
