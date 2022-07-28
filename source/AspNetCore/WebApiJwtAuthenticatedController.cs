using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api controller base class
/// </summary>
public class WebApiJwtAuthenticatedController<TWebApiClaims> : WebApiController
    where TWebApiClaims : WebApiClaims, new()
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
    public TWebApiClaims? Claims => _webApiAuthenticatedClaims ??= HttpContext.GetWebApiAuthenticatedClaims<TWebApiClaims>();

    private TWebApiClaims? _webApiAuthenticatedClaims;
}
