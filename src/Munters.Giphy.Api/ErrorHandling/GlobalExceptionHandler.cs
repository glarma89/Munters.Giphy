using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Munters.Giphy.Api.Exceptions;

namespace Munters.Giphy.Api.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException &&
            httpContext.RequestAborted.IsCancellationRequested)
        {
            return false;
        }

        var problemDetails = exception switch
        {
            GiphyApiException giphyException =>
                CreateGiphyProblemDetails(
                    httpContext,
                    giphyException),

            TaskCanceledException =>
                CreateTimeoutProblemDetails(httpContext),

            HttpRequestException =>
                CreateGiphyConnectionProblemDetails(httpContext),

            _ =>
                CreateInternalServerProblemDetails(httpContext)
        };

        LogException(exception, problemDetails.Status);

        httpContext.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
    }

    private static ProblemDetails CreateGiphyProblemDetails(
        HttpContext httpContext,
        GiphyApiException exception)
    {
        return CreateProblemDetails(
            httpContext,
            StatusCodes.Status502BadGateway,
            "Giphy API error",
            "The external GIF service returned an unsuccessful response.",
            new Dictionary<string, object?>
            {
                ["upstreamStatusCode"] =
                    (int)exception.UpstreamStatusCode
            });
    }

    private static ProblemDetails CreateTimeoutProblemDetails(
        HttpContext httpContext)
    {
        return CreateProblemDetails(
            httpContext,
            StatusCodes.Status504GatewayTimeout,
            "Giphy API timeout",
            "The external GIF service did not respond in time.");
    }

    private static ProblemDetails CreateGiphyConnectionProblemDetails(
        HttpContext httpContext)
    {
        return CreateProblemDetails(
            httpContext,
            StatusCodes.Status502BadGateway,
            "Giphy API unavailable",
            "The application could not connect to the external GIF service.");
    }

    private static ProblemDetails CreateInternalServerProblemDetails(
        HttpContext httpContext)
    {
        return CreateProblemDetails(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            "An unexpected error occurred.");
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int status,
        string title,
        string detail,
        IDictionary<string, object?>? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        if (extensions is not null)
        {
            foreach (var extension in extensions)
            {
                problemDetails.Extensions[extension.Key] =
                    extension.Value;
            }
        }

        return problemDetails;
    }

    private void LogException(
        Exception exception,
        int? statusCode)
    {
        if (statusCode is
            StatusCodes.Status502BadGateway or
            StatusCodes.Status504GatewayTimeout)
        {
            _logger.LogWarning(
                exception,
                "External service request failed with response status {StatusCode}",
                statusCode);

            return;
        }

        _logger.LogError(
            exception,
            "Unhandled exception occurred with response status {StatusCode}",
            statusCode);
    }
}