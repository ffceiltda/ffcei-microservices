using Microsoft.AspNetCore.Builder;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public static class ShuttingDownHandlerMiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseShuttingDownHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ShuttingDownHandlerMiddleware>();
        }
    }
}
