using FFCEI.Microservices.AspNetCore.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    /// <summary>
    /// Middleware for handling post-JWT validation custom authorization rules
    /// </summary>
    public sealed class JwtPostAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtPostAuthorizationDelegateMethod? _delegateMethod;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Next middleware</param>
        /// <param name="options">Options</param>
        /// <exception cref="ArgumentNullException">throw if options is null</exception>
        public JwtPostAuthorizationMiddleware(RequestDelegate next, IOptions<JwtPostAuthorizationMiddlewareOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _delegateMethod = options.Value.JwtPostAuthorization;
        }

        /// <summary>
        /// Handle a HTTP request
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Continuation</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var authorized = false;

            var controllerActionDescriptor = httpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
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
                var authenticated = httpContext.User?.Identity?.IsAuthenticated ?? false;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                if (authenticated)
                {
                    authorized = (_delegateMethod is null) ? authenticated : await _delegateMethod(httpContext);
                }
            }

            if (authorized)
            {
                await _next(httpContext);
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailStatusUnauthorized));
            }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
    }
}
