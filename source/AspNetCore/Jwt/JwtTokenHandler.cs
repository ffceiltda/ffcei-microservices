using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FFCEI.Microservices.AspNetCore.Jwt;

/// <summary>
/// Javascript Web Token handler helper class
/// </summary>
public static class JwtTokenHandler
{
    internal static DateTime MaximumTokenExpiration = new DateTime(2038, 1, 19, 3, 14, 07, DateTimeKind.Utc);

    /// <summary>
    /// Create a security Javascript Web Token
    /// </summary>
    /// <param name="expiration">Expires after</param>
    /// <param name="subjectClaims">Claims</param>
    /// <param name="signingCredentials">Signing key</param>
    /// <param name="roles">Claim roles</param>
    /// <param name="encryptingCredentials">Encrypting credentials</param>
    /// <param name="issuer">Issuer</param>
    /// <param name="audience">Audience</param>
    /// <returns>SecurityToken instance</returns>
    public static JwtSecurityToken CreateJwtSecurityToken(ref TimeSpan? expiration, IEnumerable<KeyValuePair<string, string>>? subjectClaims = null, SigningCredentials? signingCredentials = null, IEnumerable<string>? roles = null, EncryptingCredentials? encryptingCredentials = null, string? issuer = null, string? audience = null)
    {
        if (subjectClaims is null)
        {
            throw new ArgumentNullException(nameof(subjectClaims));
        }

        if (signingCredentials is null)
        {
            throw new ArgumentNullException(nameof(signingCredentials));
        }

        DateTime issuedAt = DateTime.UtcNow;
        DateTime? expiresAt = expiration is not null ? issuedAt + expiration : null;

        if ((expiresAt is not null) && (expiresAt > MaximumTokenExpiration))
        {
            expiresAt = MaximumTokenExpiration;
            expiration = expiresAt - issuedAt;
        }

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
                if (expiresAt.HasValue)
                {
                    claimValue = expiresAt.Value.ToString(CultureInfo.InvariantCulture);
                }
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
        var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(jwtTokenDescriptor);

        return jwtToken;
    }

    /// <summary>
    /// Get security token string representation
    /// </summary>
    /// <param name="jwtToken">Jwt Security Token</param>
    /// <param name="bearerTokenPrefix">Bearer token string prefix</param>
    /// <returns></returns>
    public static string BearerToken(this JwtSecurityToken jwtToken, string? bearerTokenPrefix = null)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var jwtTokenStringBuilder = new StringBuilder();

#pragma warning disable IDE0058 // Expression value is never used
        if (!string.IsNullOrEmpty(bearerTokenPrefix))
        {
            jwtTokenStringBuilder.Append(bearerTokenPrefix);
            jwtTokenStringBuilder.Append(' ');
        }

        var jwtTokenString = jwtTokenHandler.WriteToken(jwtToken);

        jwtTokenStringBuilder.Append(jwtTokenString);
#pragma warning restore IDE0058 // Expression value is never used

        return jwtTokenStringBuilder.ToString();
    }
}
