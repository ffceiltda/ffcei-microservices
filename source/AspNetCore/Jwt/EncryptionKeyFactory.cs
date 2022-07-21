using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Security;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    public sealed class EncryptionKeyFactory : SecurityKeyFactory
    {
        public EncryptionKeyFactory(ConfigurationManager configurationManager, ILogger? logger = null)
            : base(configurationManager, "Jwt.Encryption.", logger)
        {
        }
    }
}
