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
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        if (WebApiMicroservice.Instance?.ShuttingDown ?? true)
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            httpContext.Response.ContentType = "text/plain";

            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Service is not available"));

            return;
        }

        await _next(httpContext);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
