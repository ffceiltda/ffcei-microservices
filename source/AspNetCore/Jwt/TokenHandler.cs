using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FFCEI.Microservices.AspNetCore.Jwt
{
    /// <summary>
    /// Javascript Web Token handler helper class
    /// </summary>
    public static class TokenHandler
    {
        /// <summary>
        /// Create a Security Token
        /// </summary>
        /// <param name="expiration">Expires after</param>
        /// <param name="subjectClaims">Claims</param>
        /// <param name="signingCredentials">Signing key</param>
        /// <param name="roles">Claim roles</param>
        /// <param name="encryptingCredentials">Encrypting credentials</param>
        /// <param name="issuer">Issuer</param>
        /// <param name="audience">Audience</param>
        /// <returns>SecurityToken instance</returns>
        public static SecurityToken CreateToken(TimeSpan expiration, IEnumerable<KeyValuePair<string, string>>? subjectClaims = null, SigningCredentials? signingCredentials = null, IEnumerable<string>? roles = null, EncryptingCredentials? encryptingCredentials = null, string? issuer = null, string? audience = null)
        {
            if (subjectClaims == null)
            {
                throw new ArgumentNullException(nameof(subjectClaims));
            }

            if (signingCredentials == null)
            {
                throw new ArgumentNullException(nameof(signingCredentials));
            }

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddTicks(expiration.Ticks);
            var jwtSujectClaims = new ClaimsIdentity();

            foreach (var claim in subjectClaims)
            {
                string claimValue = claim.Value;

                if (claim.Key == JwtRegisteredClaimNames.Jti)
                {
                    claimValue = Guid.NewGuid().ToString();
                }
                else if (claim.Key == JwtRegisteredClaimNames.Iss)
                {
                    if (!string.IsNullOrEmpty(issuer))
                    {
                        claimValue = issuer;
                    }
                }
                else if (claim.Key == JwtRegisteredClaimNames.Aud)
                {
                    if (!string.IsNullOrEmpty(audience))
                    {
                        claimValue = audience;
                    }
                }
                else if (claim.Key == JwtRegisteredClaimNames.Iat)
                {
                    claimValue = issuedAt.ToString(CultureInfo.InvariantCulture);
                }
                else if (claim.Key == JwtRegisteredClaimNames.Nbf)
                {
                    claimValue = issuedAt.ToString(CultureInfo.InvariantCulture);
                }
                else if (claim.Key == JwtRegisteredClaimNames.Exp)
                {
                    claimValue = expiresAt.ToString(CultureInfo.InvariantCulture);
                }

                var jwtClaim = new Claim(claim.Key, claimValue);

                jwtSujectClaims.AddClaim(jwtClaim);
            }

            var jwtTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = expiresAt,
                TokenType = "Bearer",
                Subject = jwtSujectClaims,
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials
            };

            if (roles is not null)
            {
                foreach (var role in roles)
                {
                    jwtTokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            return jwtTokenHandler.CreateToken(jwtTokenDescriptor);
        }

        public static string BearerTokenString(this SecurityToken securityToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtTokenString = jwtTokenHandler.WriteToken(securityToken);

            return jwtTokenString;
        }
    }
}
