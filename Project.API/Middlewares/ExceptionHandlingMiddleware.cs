using Project.Application.Exceptions;
using Project.Common.Constants;
using System.Text.Json;

namespace Project.API.Middlewares
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
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(context, ex);
            }
        }
        private async Task HandleErrorAsync(HttpContext context, Exception ex)
        {
            string message;
            int statusCode;
            string errorCode = "UNKNOWN";
            string errorType = ex.GetType().Name;
            if (ex is BaseCustomException)
            {
                BaseCustomException baseCustomException = (BaseCustomException)ex;
                message = baseCustomException.Message;
                statusCode = (int)baseCustomException.HttpStatusCode;
                errorCode = baseCustomException.ErrorCode;
                errorType = baseCustomException.ErrorType;
            }
            else
            {
                message = ErrorMessages.SystemError;
                statusCode = 500;
            }
            _logger.LogError(ex, "Error occurred. StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, ErrorType: {ErrorType}, Message: {Message}", statusCode, errorCode, errorType, message);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response = new
            {
                success = false,
                message = message,
                error = new
                {
                    code = errorCode,
                    type = errorType
                }
            };
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}