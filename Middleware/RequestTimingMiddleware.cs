using System.Diagnostics;

namespace DemoAPINET.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watch = Stopwatch.StartNew();

            await _next(context); // Chờ Request chạy xong

            watch.Stop();

            _logger.LogInformation(
                "Request {Method} {Path} mất {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                watch.ElapsedMilliseconds);
        }
    }
}
