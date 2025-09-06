using System.Net;
using ecommapi.Application.Contracts;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace ecommapi.Infrastructure.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred");
            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                Title = exception.GetType().Name,
                TimeStamp = DateTime.UtcNow
            };
            switch (exception)
            {
                case BadHttpRequestException:
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case UserNotFoundException:
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;

            }
            httpContext.Response.StatusCode = errorResponse.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}