using System.Net;

namespace DemoAPINET.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Chạy tiếp pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi hệ thống xảy ra");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "Đã có lỗi xảy ra từ máy chủ, vui lòng thử lại sau.",
                detail = ex.Message // Chỉ nên log, cân nhắc không gửi ra cho client môi trường Prod
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
