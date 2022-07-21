using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Security;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    public sealed class SigningKeyFactory : SecurityKeyFactory
    {
        public SigningKeyFactory(ConfigurationManager configurationManager, ILogger? logger = null)
            : base(configurationManager, "Jwt.Signing.", logger)
        {
        }
    }
}
