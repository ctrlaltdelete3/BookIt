using BookIt.Application.DTOs;
using System.Net;
using System.Text.Json;

namespace BookIt.Api.Middleware
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
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        public Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string errorMessage = string.Empty;

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorMessage = "The requested resource was not found.";
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorMessage = "You do not have access to this resource.";
                    break;
                case ArgumentNullException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = "Required parameter is missing.";
                    break;
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = exception.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorMessage = "An error occurred.";
                    break;
            }

            var response = new ExceptionResponseDto
            {
                Message = errorMessage,
                StatusCode = (int)statusCode,
                Details = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(jsonResponse);
        }

    }
}
