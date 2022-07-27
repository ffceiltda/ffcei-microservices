using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api claims base class
/// </summary>
public static class WebApiClaimsExtensionMethods
{
    /// <summary>
    /// Get Web Api Authenticated Claims
    /// </summary>
    /// <typeparam name="TWebApiClaims">WebApiClaims descendant type</typeparam>
    /// <param name="httpContext">ASP.NET Http Context</param>
    /// <returns>TWebApiClaims instance</returns>
    public static TWebApiClaims? GetWebApiAuthenticatedClaims<TWebApiClaims>(this HttpContext httpContext)
        where TWebApiClaims : WebApiClaims, new()
    {
        if ((httpContext?.User.Identity ?? null) is not ClaimsIdentity claims)
        {
            return null;
        }

        if (!claims.IsAuthenticated)
        {
            return null;
        }

        var result = new TWebApiClaims();

        result.ParseClaims(claims);

        return result;
    }
}
