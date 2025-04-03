using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TestWorkForModsen.Core.Exceptions;

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
            _logger.LogInformation("Вызывается ExceptionHandlingMiddleware");
            try
            {
                await _next(context);
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    throw new CustomNotFoundException("Ресурс не найден");
                }
                else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    throw new CustomUnauthorizedException("Требуется авторизация");
                }
                _logger.LogInformation("ExceptionHandlingMiddleware ничего не поймал");
            }
            catch (CustomValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (CustomBadRequestException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomNotFoundException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomConflictException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomForbiddenException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomUnauthorizedException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex) // Стандартное исключение
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HandleApiExceptionAsync(context,
                    new CustomUnauthorizedException(ex.Message));
            }
            catch (CustomDatabaseException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomTokenGenerationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (CustomServiceUnavailableException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await HandleApiExceptionAsync(context, ex);
            }
            catch (ApiException ex)
            {
                await HandleApiExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private async Task HandleValidationExceptionAsync(HttpContext context, CustomValidationException exception)
        {
            _logger.LogError(exception, "Validation Exception: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Errors = exception.Errors,
                Details = exception.InnerException?.Message
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }

        private async Task HandleApiExceptionAsync(HttpContext context, ApiException exception)
        {
            _logger.LogError(exception, "API Exception: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Details = exception.InnerException?.Message
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }

        private async Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error",
                Details = exception.Message
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }
    }
}