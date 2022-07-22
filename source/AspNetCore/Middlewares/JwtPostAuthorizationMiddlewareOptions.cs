using FFCEI.Microservices.AspNetCore.Jwt;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    /// <summary>
    /// JwtPostAuthorizationMiddleware options
    /// </summary>
    public sealed class JwtPostAuthorizationMiddlewareOptions
    {
        /// <summary>
        /// Javascript Web Token: late authorization delegate method
        /// </summary>
        public JwtPostAuthorizationDelegateMethod? JwtPostAuthorization { get; set; }
    }
}
