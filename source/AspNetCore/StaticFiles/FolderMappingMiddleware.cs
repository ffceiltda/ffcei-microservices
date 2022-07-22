using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.StaticFiles
{
    public sealed class FolderMappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FolderMappingMiddleware> _logger;
        private readonly SortedDictionary<string, FolderMapping> _staticFolderMappings;

        public FolderMappingMiddleware(RequestDelegate next, ILogger<FolderMappingMiddleware> logger, SortedDictionary<string, FolderMapping> staticFolderMappings)
        {
            _next = next;
            _logger = logger;
            _staticFolderMappings = staticFolderMappings;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (httpContext.Request.Method == "GET")
            {
                var requestPath = httpContext.Request.Path.ToString();
                var requestAuthorized = false;

                foreach (var mapping in _staticFolderMappings.Values)
                {
                    if (requestAuthorized)
                    {
                        break;
                    }

                    if (requestPath.StartsWith(mapping.WebPath, StringComparison.InvariantCulture))
                    {
                        switch (mapping.AuthorizationPolicy)
                        {
                        case StaticFolderMappingAuthorizationPolicy.PublicAccess:
                            {
                                requestAuthorized = true;

                                continue;
                            }
                        case StaticFolderMappingAuthorizationPolicy.AuthenticatedAccess:
                            {
                                if (await CheckAuthentication(httpContext))
                                {
                                    requestAuthorized = true;

                                    continue;
                                }

                                return;
                            }
                        case StaticFolderMappingAuthorizationPolicy.AuthorizedRoles:
                            {
                                if (await CheckAuthorizedRoles(httpContext, mapping.AuthorizedRoles))
                                {
                                    requestAuthorized = true;

                                    continue;
                                }

                                return;
                            }
                        default:
                            {
                                break;
                            }
                        }
                    }
                }
            }

#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable CA1848 // Use the LoggerMessage delegates
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger?.LogError(e, "Unauthorized access to static file requested at {Path}", httpContext.Request.Path);

                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailStatusForbidden));
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Exception caught while serving static file requested at {Path}", httpContext.Request.Path);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(WebApiResultBase.DetailInternalError));
            }
#pragma warning restore CA1848 // Use the LoggerMessage delegates
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static async Task<bool> CheckAuthorizedRoles(HttpContext httpContext, HashSet<string>? authorizedRoles = null)
        {
            if (await CheckAuthentication(httpContext))
            {
                if (authorizedRoles is not null)
                {
                    bool isInRole = false;

                    foreach (var role in authorizedRoles)
                    {
                        if (httpContext.User.IsInRole(role))
                        {
                            isInRole = true;

                            break;
                        }
                    }

                    if (!isInRole)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                        httpContext.Response.ContentType = "text/plain";

                        await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"You are not allowed to access {httpContext.Request.Path} on this server"));
                    }
                }
            }

            return true;
        }

        private static async Task<bool> CheckAuthentication(HttpContext httpContext)
        {
            if (httpContext.User.Identity is not { IsAuthenticated: true })
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"You are not authorized to access {httpContext.Request.Path} on this server"));

                return false;
            }

            return true;
        }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
