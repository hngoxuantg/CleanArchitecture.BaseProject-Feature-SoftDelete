using System.Text;

namespace MotorbikeRental.Web.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestResponseLoggingMiddleware> logger;
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            LogRequest(context.Request);
            await next(context);
            LogResponse(context.Response);
        }
        private void LogRequest(HttpRequest request)
        {
            logger.LogInformation($"Request received: {request.Method} {request.Path}");
            logger.LogInformation($"Request headers: {GetHeadersAsString(request.Headers)}");
        }
        private void LogResponse(HttpResponse response)
        {
            logger.LogInformation($"Response sent: {response.StatusCode}");
            logger.LogInformation($"Response headers: {GetHeadersAsString(response.Headers)}");
        }
        private string GetHeadersAsString(IHeaderDictionary headers)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                stringBuilder.AppendLine($"{key}: {value}");
            }
            return stringBuilder.ToString();
        }
    }
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}