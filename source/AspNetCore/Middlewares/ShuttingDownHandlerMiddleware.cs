using FFCEI.Microservices.Microservices;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares;

/// <summary>
/// Gracefull shutdown middleware (interrupt request serving while server is shutting down...)
/// </summary>
public sealed class ShuttingDownHandlerMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Next middleware</param>
    public ShuttingDownHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Handle a HTTP request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Continuation</returns>
    /// <exception cref="ArgumentNullException">throws if httpContext is null</exception>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));

        if (Microservice.Instance?.ShuttingDown ?? true)
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            httpContext.Response.ContentType = "text/plain";

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Service is not available"));
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await _next(httpContext);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
