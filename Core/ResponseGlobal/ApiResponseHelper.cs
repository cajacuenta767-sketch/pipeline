namespace Core.ResponseGlobal
{
	public static class ApiResponseHelper
	{
		public static ApiResponseGlobal<T> Success<T>(T data, string message = "Operación exitosa")
		{
			return new ApiResponseGlobal<T>
			{
				Success = true,
				Message = message,
				Data = data,
				StatusCode = 200
			};
		}

		public static ApiResponseGlobal<object> Error(string message, int statusCode = 400, object errors = null)
		{
			return new ApiResponseGlobal<object>
			{
				Success = false,
				Message = message,
				Data = null,
				StatusCode = statusCode,
				Errors = errors
			};
		}
	}
}
