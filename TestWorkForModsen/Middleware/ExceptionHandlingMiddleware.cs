using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TestWorkForModsen.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Сохраняем оригинальный поток ответа
            var originalBodyStream = context.Response.Body;

            try
            {
                // Используем буфер для перехвата ответа
                using var responseBuffer = new MemoryStream();
                context.Response.Body = responseBuffer;

                await _next(context);

                // Если статус код не 2xx - обрабатываем как ошибку
                if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                {
                    await HandleErrorResponseAsync(context);
                }

                // Возвращаем буферизированный ответ
                responseBuffer.Seek(0, SeekOrigin.Begin);
                await responseBuffer.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                // В случае исключения возвращаем оригинальный поток
                context.Response.Body = originalBodyStream;
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleErrorResponseAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetStatusCodeMessage(context.Response.StatusCode)
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error",
                Details = exception.Message
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse);
        }

        private static string GetStatusCodeMessage(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            _ => $"Error: {statusCode}"
        };
    }
}