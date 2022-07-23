using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings
{
    /// <summary>
    /// StaticFolderMappingMiddleware extension methods
    /// </summary>
    public static class StaticFolderMappingMiddlewareExtensionMethods
    {
        /// <summary>
        /// Add JwtPostAuthorizationMiddleware to ServiceCollection
        /// </summary>
        /// <param name="service">ServiceCollection instance</param>
        /// <param name="options">Options</param>
        /// <returns>ServiceCollection instance</returns>
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static IServiceCollection AddStaticFolderMappings(this IServiceCollection service, Action<StaticFolderMappingMiddlewareOptions> options = default)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
#pragma warning disable IDE0054 // Use compound assignment
            options = options ?? (opts => { });
#pragma warning restore IDE0054 // Use compound assignment

#pragma warning disable IDE0058 // Expression value is never used
            service.Configure(options);
#pragma warning restore IDE0058 // Expression value is never used

            return service;
        }

        /// <summary>
        /// Use JwtPostAuthorizationMiddleware middleware in ApplicationBuilder
        /// </summary>
        /// <param name="builder">ApplicationBuilder instance</param>
        /// <returns>ApplicationBuilder instance</returns>
        public static IApplicationBuilder UseStaticFolderMappings(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StaticFolderMappingMiddleware>();
        }
    }
}
