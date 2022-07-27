using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Security;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore.Jwt;

/// <summary>
/// Javascript Web Token signing key factory
/// </summary>
public sealed class JwtSigningKeyFactory : SecurityKeyFactory
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configurationManager">ConfigurationManager</param>
    /// <param name="logger">Loggert</param>
    public JwtSigningKeyFactory(ConfigurationManager configurationManager, ILogger? logger = null)
        : base(configurationManager, "Jwt.Signing.", logger)
    {
    }
}
