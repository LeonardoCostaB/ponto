using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Ponto.Api.Infra.Results;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, result) = exception switch
        {
            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                Result.Fail(validation.Errors
                    .Select(error => new ResultError(error.PropertyName, error.ErrorMessage))
                    .ToArray())),
            _ => (
                StatusCodes.Status500InternalServerError,
                Result.Fail(new ResultError("server", "An unexpected error occurred.")))
        };

        if (statusCode == StatusCodes.Status400BadRequest)
        {
            _logger.LogWarning(exception, "Request validation failed");
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}
