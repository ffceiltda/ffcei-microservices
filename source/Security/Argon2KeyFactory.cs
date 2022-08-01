using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Generators;
using System.Globalization;

namespace FFCEI.Microservices.Security
{
    public sealed class Argon2KeyFactory
    {
        private string? _secretKey;
        private int _defaultSaltLength = 64;

        public Argon2KeyFactory(IConfigurationManager configurationManager)
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

            if (_defaultSaltLength < 64)
            {
                throw new InvalidOperationException("Argon2.Salt.Length must be at least 64");
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

            return RandomTokenGenerator.GenerateRandomToken(length, true, true, LetterCapitalization.AnyCase, true);
        }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string? SecretKey => _secretKey;
    }
}
