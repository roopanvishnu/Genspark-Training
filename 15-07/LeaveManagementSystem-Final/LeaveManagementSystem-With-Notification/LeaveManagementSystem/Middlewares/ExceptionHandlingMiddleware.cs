using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using log4net;

namespace LeaveManagementSystem.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ExceptionHandlingMiddleware));

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            LogRequest(context);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogException(context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private void LogRequest(HttpContext context)
        {
            var userId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : "Anonymous";

            _logger.Info($"[Incoming Request] Timestamp: {DateTime.UtcNow:O}, " +
                         $"Method: {context.Request.Method}, " +
                         $"Path: {context.Request.Path}, " +
                         $"User: {userId}");
        }

        private void LogException(HttpContext context, Exception ex)
        {
            var userId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : "Anonymous";

            _logger.Error($"[Exception Caught] Timestamp: {DateTime.UtcNow:O}, " +
                          $"Path: {context.Request.Path}, " +
                          $"Method: {context.Request.Method}, " +
                          $"User: {userId}, " +
                          $"Exception: {ex}");
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is UnauthorizedAccessException)
                statusCode = HttpStatusCode.Unauthorized;

            var errorResponse = new
            {
                StatusCode = (int)statusCode,
                Message = exception.Message
            };

            string errorJson = System.Text.Json.JsonSerializer.Serialize(errorResponse);

            LogManager.GetLogger(typeof(ExceptionHandlingMiddleware))
                      .Info($"[Exception Response] StatusCode: {(int)statusCode}, Message: {exception.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(errorJson);
        }
    }
}
