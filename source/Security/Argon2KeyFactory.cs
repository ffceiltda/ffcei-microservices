using FFCEI.Microservices.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using Isopoh.Cryptography.Argon2;

namespace FFCEI.Microservices.Security
{
    public sealed class Argon2KeyFactory
    {
        private string? _secretKey;
        private int _defaultSaltLength = 64;

        public Argon2KeyFactory(ConfigurationManager configurationManager)
        {
            if (configurationManager is null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            _secretKey = configurationManager["Argon2.Secret"];

            var saltLength = configurationManager["Argon2.Salt.Length"];

            if (saltLength is not null)
            {
                _defaultSaltLength = int.Parse(saltLength, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Generate a salt with n bytes
        /// </summary>
        /// <param name="saltLength">Salt length</param>
        /// <returns>Salt generated</returns>
        public string GenerateSalt(int? saltLength = null)
        {
            var length = saltLength ?? _defaultSaltLength;

            if (length < _defaultSaltLength)
            {
                length = _defaultSaltLength;
            }

            var bytes = new byte[length];

            RandomNumberGenerator.Fill(bytes);

            for (int i = 0; i < saltLength; i++)
            {
                bytes[i] = (byte)((bytes[i] % 93) + 33);
            }

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string? SecretKey => _secretKey;
    }
}
