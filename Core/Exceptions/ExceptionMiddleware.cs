using Core.ResponseGlobal;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Core.Exceptions
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context,	Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			var response = ApiResponseHelper.Error(
				"Ocurrió un error interno en el servidor.",
				StatusCodes.Status500InternalServerError,
				exception.Message);

			var json = JsonSerializer.Serialize(response);

			await context.Response.WriteAsync(json);
		}
	}
}
