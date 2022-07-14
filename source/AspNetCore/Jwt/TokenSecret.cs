using FFCEI.Microservices.Configuration;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    sealed class TokenSecret
    {
        public string? Key { get; set; }
        public byte[]? KeyBytes => string.IsNullOrEmpty(Key) ? null : Encoding.UTF8.GetBytes(Key);

        public TokenSecret(ConfigurationManager configurationManager)
        {
            Key = configurationManager["Jwt.Token"];
        }
    }
}
