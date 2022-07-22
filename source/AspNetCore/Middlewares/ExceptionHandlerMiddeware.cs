using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public sealed class ExceptionHandlerMiddeware
    {
        private readonly ILogger<ExceptionHandlerMiddeware> _logger;

        public ExceptionHandlerMiddeware(ILogger<ExceptionHandlerMiddeware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context is not null)
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                _logger.LogError("Exception caught on middleware pipeline, replying with internal server error");
#pragma warning restore CA1848 // Use the LoggerMessage delegates

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailInternalError));
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            }
        }
    }
}
