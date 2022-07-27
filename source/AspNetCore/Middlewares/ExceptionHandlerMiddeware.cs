using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares;

/// <summary>
/// Middleware for filtering unhandled exceptions, logging and return internal error response
/// </summary>
public sealed class ExceptionHandlerMiddeware
{
    private readonly ILogger<ExceptionHandlerMiddeware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    public ExceptionHandlerMiddeware(ILogger<ExceptionHandlerMiddeware> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handle a HTTP request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Continuation</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext is not null)
        {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogError("Exception caught on middleware pipeline, replying with internal server error");
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "text/plain";

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailInternalError));
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
    }
}
