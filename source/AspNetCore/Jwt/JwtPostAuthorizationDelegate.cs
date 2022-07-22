using Microsoft.AspNetCore.Http;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    /// <summary>
    /// Javascript Web Token: late authorization delegate method
    /// </summary>
    /// <param name="httpContext">ASP.NET Core HTTP Context</param>
    /// <returns>true to continue, false otherwise</returns>
    public delegate Task<bool> JwtPostAuthorizationDelegateMethod(HttpContext httpContext);
}
