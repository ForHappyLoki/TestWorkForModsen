using System.Text.Json;

namespace TestWorkForModsen.Middleware
{
    //Это мидлваре отрабатывает, когда мы не находим страницу и получаем 404
    public class NotFoundMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json"; 
                var response = new
                {
                    error = "Страница не найдена.",
                    path = context.Request.Path 
                };
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                var jsonResponse = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
