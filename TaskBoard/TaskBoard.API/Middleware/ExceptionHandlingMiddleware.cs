using System.Net;
using System.Text.Json;

namespace TaskBoard.API.Middleware
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
            try
            {
                await _next(context);

                // ✅ Handle 4xx responses (BadRequest, Unauthorized, Forbidden, NotFound, etc.)
                if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
                {
                    var correlationId = EnsureCorrelationId(context);

                    _logger.LogWarning(
                        "Client error | Path: {Path} | StatusCode: {StatusCode} | CorrelationId: {CorrelationId} | UserId: {UserId}",
                        context.Request.Path,
                        context.Response.StatusCode,
                        correlationId,
                        context.User?.Identity?.Name ?? "anonymous"
                    );

                    if (!context.Response.HasStarted)
                    {
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            error = $"Client error ({context.Response.StatusCode})",
                            correlationId
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                }
            }
            catch (Exception ex)
            {
                var correlationId = EnsureCorrelationId(context);

                _logger.LogError(ex,
                    "Unhandled exception | Path: {Path} | CorrelationId: {CorrelationId} | UserId: {UserId}",
                    context.Request.Path,
                    correlationId,
                    context.User?.Identity?.Name ?? "anonymous"
                );

                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    error = "An unexpected error occurred.",
                    correlationId
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }

        private static string EnsureCorrelationId(HttpContext context)
        {
            if (!context.Response.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
                string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Response.Headers["X-Correlation-ID"] = correlationId;
            }

            return correlationId!;
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
