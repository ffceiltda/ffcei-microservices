using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Security;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore.Jwt;

/// <summary>
/// Javascript Web Token encryption key factory
/// </summary>
public sealed class JwtEncryptionKeyFactory : SecurityKeyFactory
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configurationManager">ConfigurationManager</param>
    /// <param name="logger">Logger</param>
    public JwtEncryptionKeyFactory(IConfigurationManager configurationManager, ILogger? logger = null)
        : base(configurationManager, "Jwt.Encryption.", logger)
    {
    }
}
