using System.Security.Claims;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using FFCEI.Microservices.AspNetCore.Jwt;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api claims base class
    /// </summary>
    public class WebApiClaims
    {
        private SortedSet<string> _roles = new();

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebApiClaims()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="claims">Claims to be parsed</param>
        public WebApiClaims(ClaimsIdentity claims)
        {
            DoParseClaims(this, claims);
        }

        /// <summary>
        /// Issued At
        /// </summary>
        [WebApiClaim(Type = "iat")]
        public DateTimeOffset? IssuedAt { get; set; }

        /// <summary>
        /// Expires At
        /// </summary>
        [WebApiClaim(Type = "exp")]
        public DateTimeOffset? ExpiresAt { get; set; }

        /// <summary>
        /// Not Before
        /// </summary>
        [WebApiClaim(Type = "nbf")]
        public DateTimeOffset? NotBefore { get; set; }

        private void DoParseClaims(object instance, ClaimsIdentity claims)
        {
            if (claims is null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var instanceClaims = instance.GetType().GetProperties().Where(claim => claim.GetCustomAttributes(typeof(WebApiClaimAttribute), true).Length > 0).ToList();

            foreach (var instanceClaim in instanceClaims)
            {
                var claimType = instanceClaim.Name;
                var claim = claims.FindFirst(c => c.Type == claimType);

                if (claim is not null)
                {
                    if ((instanceClaim.PropertyType == typeof(Guid)) || (instanceClaim.PropertyType == typeof(Guid?)))
                    {
                        var guidValue = Guid.Parse(claim.Value);

                        instanceClaim.SetValue(instance, guidValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(int)) || (instanceClaim.PropertyType == typeof(int?)))
                    {
                        var intValue = int.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, intValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(uint)) || (instanceClaim.PropertyType == typeof(uint?)))
                    {
                        var intValue = uint.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, intValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(long)) || (instanceClaim.PropertyType == typeof(long?)))
                    {
                        var longValue = long.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, longValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(ulong)) || (instanceClaim.PropertyType == typeof(ulong?)))
                    {
                        var longValue = ulong.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, longValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(bool)) || (instanceClaim.PropertyType == typeof(bool?)))
                    {
                        var boolValue = bool.Parse(claim.Value);

                        instanceClaim.SetValue(instance, boolValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(DateTime)) || (instanceClaim.PropertyType == typeof(DateTime?)))
                    {
                        var dateTimeValue = DateTime.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, dateTimeValue);
                    }
                    else if ((instanceClaim.PropertyType == typeof(DateTimeOffset)) || (instanceClaim.PropertyType == typeof(DateTimeOffset?)))
                    {
                        var dateTimeOffsetValue = DateTimeOffset.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, dateTimeOffsetValue);
                    }
                    else if (instanceClaim.PropertyType == typeof(string))
                    {
                        var stringValue = claim.Value;

                        instanceClaim.SetValue(instance, stringValue);
                    }
                    else
                    {
                        var attribute = instanceClaim.GetCustomAttributes(typeof(WebApiClaimAttribute), true).FirstOrDefault();

                        if (attribute is null)
                        {
                            throw new InvalidOperationException($"Missing claim attribute for {claimType}");
                        }

                        var webApiClaimAttribute = (attribute as WebApiClaimAttribute);

                        if (webApiClaimAttribute is null)
                        {
                            throw new InvalidOperationException($"Invalid claim attribute for {claimType}");
                        }

                        if (webApiClaimAttribute.Required)
                        {
                            throw new InvalidOperationException($"Missing required claim value for {claimType}");
                        }
                    }
                }
            }

            var allClaims = claims.FindAll(c => c.Type == ClaimTypes.Role).ToList();

            foreach (var claim in allClaims)
            {
#pragma warning disable IDE0058 // Expression value is never used
                _roles.Add(claim.Value);
#pragma warning restore IDE0058 // Expression value is never used
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        public void ParseClaims(ClaimsIdentity claims)
        {
            DoParseClaims(this, claims);
        }

        private List<KeyValuePair<string, string>> GetSubjectClaims()
        {
            return DoGetSubjectClaims(this);
        }

        private static List<KeyValuePair<string, string>> DoGetSubjectClaims(object instance)
        {
            var result = new List<KeyValuePair<string, string>>();

            var instanceClaims = instance.GetType().GetProperties().Where(claim => claim.GetCustomAttributes(typeof(WebApiClaimAttribute), true).Length > 0).ToList();

            foreach (var instanceClaim in instanceClaims)
            {
                var claimType = instanceClaim.Name;
                var claimValue = instanceClaim.GetValue(instance)?.ToString();

                if (string.IsNullOrEmpty(claimValue))
                {
                    continue;
                }

                result.Add(new KeyValuePair<string, string>(claimType, claimValue));
            }

            return result;
        }

        /// <summary>
        /// Claims
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, string>> SubjectClaims => GetSubjectClaims();

        /// <summary>
        /// Add a Role to Claims
        /// </summary>
        /// <param name="role">Role Id</param>
        public void AddRole(string role)
        {
#pragma warning disable IDE0058 // Expression value is never used
            _roles.Add(role);
#pragma warning restore IDE0058 // Expression value is never used
        }

        /// <summary>
        /// Roles
        /// </summary>
        public IReadOnlySet<string> Roles => _roles;

        /// <summary>
        /// Creates a Jwt Security Token
        /// </summary>
        /// <param name="signingCredentials">Signing key</param>
        /// <param name="encryptingCredentials">Encrypting credentials</param>
        /// <param name="issuer">Issuer</param>
        /// <param name="audience">Audience</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public JwtSecurityToken CreateJwtSecurityToken(SigningCredentials? signingCredentials = null, EncryptingCredentials? encryptingCredentials = null, string? issuer = null, string? audience = null)
        {
            IssuedAt = DateTimeOffset.Now;

            if (NotBefore is not null)
            {
                if (NotBefore < IssuedAt)
                {
                    NotBefore = IssuedAt;
                }
            }

            TimeSpan? expiration = null;

            if (ExpiresAt is not null)
            {
                if (ExpiresAt <= IssuedAt)
                {
                    throw new InvalidOperationException("Cannot issue an already expired token");
                }

                NotBefore = IssuedAt;

                expiration = ExpiresAt - IssuedAt;
            }

            var token = JwtTokenHandler.CreateJwtSecurityToken(ref expiration,
                subjectClaims: SubjectClaims,
                signingCredentials: signingCredentials,
                encryptingCredentials: encryptingCredentials,
                roles: Roles,
                issuer: issuer,
                audience: audience);

            return token;
        }
    }
}
