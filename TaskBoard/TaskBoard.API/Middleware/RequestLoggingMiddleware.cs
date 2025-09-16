using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskBoard.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate or read correlation id
            var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
                ? context.Request.Headers["X-Correlation-ID"].ToString()
                : Guid.NewGuid().ToString();

            context.Response.Headers["X-Correlation-ID"] = correlationId;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

                // Mask sensitive headers
                var headers = context.Request.Headers
                    .Where(h => h.Key != "Authorization") // skip logging token
                    .ToDictionary(h => h.Key, h => h.Value.ToString());

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    headers["Authorization"] = "****MASKED****";
                }

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms | CorrelationId: {CorrelationId} | UserId: {UserId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId,
                    userId
                );
            }
        }
    }

    // Extension method for easy registration
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
