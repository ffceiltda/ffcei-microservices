using FFCEI.Microservices.AspNetCore.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public sealed class JwtPostAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtPostAuthorizationDelegateMethod? _delegateMethod;

        public JwtPostAuthorizationMiddleware(RequestDelegate next, IOptions<JwtPostAuthorizationMiddlewareOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _delegateMethod = options.Value.JwtPostAuthorization;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var authorized = false;

            var controllerActionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
            var hasAllowAnonymousAttributeOnAction = controllerActionDescriptor?.MethodInfo?
                .GetCustomAttributes(inherit: true)?.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)) ?? false;
            var hasAllowAnonymousAttributeOnController = controllerActionDescriptor?.ControllerTypeInfo?
                .GetCustomAttributes(inherit: true)?.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)) ?? false;

            if (hasAllowAnonymousAttributeOnController || hasAllowAnonymousAttributeOnAction)
            {
                authorized = true;
            }
            else 
            {
                var authenticated = context.User?.Identity?.IsAuthenticated ?? false;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                if (authenticated)
                {
                    authorized = (_delegateMethod is null) ? authenticated : await _delegateMethod(context);
                }
            }

            if (authorized)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "text/plain";

                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailStatusUnauthorized));
            }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
    }
}
