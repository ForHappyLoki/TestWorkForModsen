using System.Net;
using System.Text.Json;

namespace TestWorkForModsen.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Проверяем статус-код ответа
                if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                {
                    await HandleStatusCodeAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошло необработанное исключение.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "Произошла ошибка на сервере.",
                details = exception.Message
            };

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(jsonResponse);
        }

        private static Task HandleStatusCodeAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var statusCode = context.Response.StatusCode;
            var statusCodeName = ((HttpStatusCode)statusCode).ToString();

            var response = new
            {
                error = "Произошла ошибка.",
                statusCode = statusCode,
                statusCodeName = statusCodeName
            };

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}