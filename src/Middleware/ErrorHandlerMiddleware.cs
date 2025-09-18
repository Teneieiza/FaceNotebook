using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FaceNoteBook.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case InvalidOperationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;
                case DbUpdateException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = "Database update failed. Please check your data.";
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = exception.Message;
                    break;
                case AccessViolationException:
                    statusCode = StatusCodes.Status403Forbidden;
                    message = "Forbidden access.";
                    break;
                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;
                case DuplicateWaitObjectException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = exception.Message;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = statusCode,
                message
            });

            return context.Response.WriteAsync(result);
        }
    }
}