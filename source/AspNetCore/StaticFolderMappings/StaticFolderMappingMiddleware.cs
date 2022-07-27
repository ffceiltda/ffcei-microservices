using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings;

/// <summary>
/// Middleware for folder mapping for static file serving
/// </summary>
public sealed class StaticFolderMappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StaticFolderMappingMiddleware> _logger;
    private readonly SortedDictionary<string, MappedFolder> _mappedFolders;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Next middleware in chain</param>
    /// <param name="logger">Logger</param>
    /// <param name="options">Folder mappings</param>
    public StaticFolderMappingMiddleware(RequestDelegate next, ILogger<StaticFolderMappingMiddleware> logger, StaticFolderMappingMiddlewareOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _next = next;
        _logger = logger;
        _mappedFolders = options.MappedFolders;
    }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
    /// <summary>
    /// Handle a HTTP request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Continuation</returns>
    /// <exception cref="ArgumentNullException">throw if httpContext is null</exception>
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

            foreach (var mapping in _mappedFolders.Values)
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
