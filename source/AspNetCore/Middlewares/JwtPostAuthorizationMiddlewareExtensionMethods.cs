using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public static class JwtPostAuthorizationMiddlewareExtensionMethods
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static IServiceCollection AddJwtPostAuthorization(this IServiceCollection service, Action<JwtPostAuthorizationMiddlewareOptions> options = default)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            options = options ?? (opts => { });

#pragma warning disable IDE0058 // Expression value is never used
            service.Configure(options);
#pragma warning restore IDE0058 // Expression value is never used

            return service;
        }

        public static IApplicationBuilder UseJwtPostAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtPostAuthorizationMiddleware>();
        }
    }
}
