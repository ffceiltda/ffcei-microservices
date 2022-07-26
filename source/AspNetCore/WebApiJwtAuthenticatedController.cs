using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Security.Claims;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api controller base class
    /// </summary>
    [Authorize]
    public class WebApiJwtAuthenticatedController<TWebApiClaims> : WebApiController
        where TWebApiClaims: WebApiClaims, new()
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Default logger</param>
        public WebApiJwtAuthenticatedController(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Remote client address
        /// </summary>
        public TWebApiClaims? AuthenticatedUserClaims => _authenticatedUserClaims ??= GetAuthenticatedUserClaims();

        private TWebApiClaims? _authenticatedUserClaims;

        private TWebApiClaims? GetAuthenticatedUserClaims()
        {
            var claims = (HttpContext.User.Identity ?? null) as ClaimsIdentity;

            if (claims is null)
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
}
