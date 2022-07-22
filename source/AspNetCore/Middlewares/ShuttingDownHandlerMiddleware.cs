using Microsoft.AspNetCore.Http;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public sealed class ShuttingDownHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ShuttingDownHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (WebApiMicroservice.Instance?.ShuttingDown ?? true)
            {
                if (context is null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "text/plain";

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Service is not available"));
            }
            else
            {
                await _next(context);
            }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
    }
}
