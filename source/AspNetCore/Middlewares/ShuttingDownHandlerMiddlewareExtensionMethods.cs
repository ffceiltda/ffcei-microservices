using Microsoft.AspNetCore.Builder;

namespace FFCEI.Microservices.AspNetCore.Middlewares;

/// <summary>
/// ShuttingDownHandlerMiddleware extension methods
/// </summary>
public static class ShuttingDownHandlerMiddlewareExtensionMethods
{
    /// <summary>
    /// Use ShuttingDownHandlerMiddleware middleware in ApplicationBuilder
    /// </summary>
    /// <param name="builder">ApplicationBuilder instance</param>
    /// <returns>ApplicationBuilder instance</returns>
    public static IApplicationBuilder UseShuttingDownHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ShuttingDownHandlerMiddleware>();
    }
}
