using FFCEI.Microservices.AspNetCore.Jwt;

namespace FFCEI.Microservices.AspNetCore.Middlewares
{
    public sealed class JwtPostAuthorizationMiddlewareOptions
    {
        public JwtPostAuthorizationDelegateMethod? JwtPostAuthorization { get; set; }
    }
}
