using Project.Application.Exceptions;
using Project.Common.Constants;
using System.Text.Json;

namespace MotorbikeRental.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleError(context, ex);
            }
        }
        private async Task HandleError(HttpContext context, Exception ex)
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
            logger.LogError(ex, "Error occurred. StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, ErrorType: {ErrorType}, Message: {Message}", statusCode, errorCode, errorType, message);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response = new
            {
                Success = false,
                Message = message,
                Error = new
                {
                    Code = errorCode,
                    Type = errorType
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