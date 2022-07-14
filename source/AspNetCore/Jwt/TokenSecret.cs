using FFCEI.Microservices.Configuration;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    class TokenSecret
    {
        public string? Key { get; set; }

        public TokenSecret(ConfigurationManager configurationManager)
        {
            Key = configurationManager["Jwt.Token"];
        }
    }
}