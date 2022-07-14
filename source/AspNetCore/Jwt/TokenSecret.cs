using FFCEI.Microservices.Configuration;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    class TokenSecret
    {
        public string Key { get; set; } = string.Empty;
        public byte[] KeyBytes => Encoding.UTF8.GetBytes(Key);

        public TokenSecret(ConfigurationManager configurationManager)
        {
            Key = configurationManager["Jwt.Token"];
        }
    }
}
