using FFCEI.Microservices.AspNetCore.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FFCEI.Microservices.AspNetCore;

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
    /// Issuer
    /// </summary>
    [WebApiClaim(Type = "iss", DoNotListOnSubjectClaims = true)]
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// Audience
    /// </summary>
    [WebApiClaim(Type = "aud", DoNotListOnSubjectClaims = true)]
    public string Audience { get; set; } = null!;

    /// <summary>
    /// Issued At
    /// </summary>
    [WebApiClaim(Type = "iat", DoNotListOnSubjectClaims = true)]
    public DateTimeOffset? IssuedAt { get; set; }

    /// <summary>
    /// Expires At
    /// </summary>
    [WebApiClaim(Type = "exp", DoNotListOnSubjectClaims = true)]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Not Before
    /// </summary>
    [WebApiClaim(Type = "nbf", DoNotListOnSubjectClaims = true)]
    public DateTimeOffset? NotBefore { get; set; }

    /// <summary>
    /// Compute the seconds before token expiration
    /// </summary>
    public long SecondsBeforeExpiration => (long)(ExpiresAt is null ? long.MaxValue :
        (ExpiresAt.Value.UtcDateTime - (IssuedAt ?? DateTimeOffset.UtcNow).UtcDateTime).TotalSeconds);

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

            claimType = webApiClaimAttribute.Type ?? instanceClaim.Name;

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
                    DateTime dateTimeValue;

                    try
                    {
                        dateTimeValue = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim.Value, CultureInfo.InvariantCulture)).DateTime;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        dateTimeValue = DateTimeOffset.Parse(claim.Value, CultureInfo.InvariantCulture).DateTime;
                    }

                    instanceClaim.SetValue(instance, dateTimeValue);
                }
                else if ((instanceClaim.PropertyType == typeof(DateTimeOffset)) || (instanceClaim.PropertyType == typeof(DateTimeOffset?)))
                {
                    DateTimeOffset dateTimeOffsetValue;

                    try
                    {
                        dateTimeOffsetValue = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim.Value, CultureInfo.InvariantCulture));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        dateTimeOffsetValue = DateTimeOffset.Parse(claim.Value, CultureInfo.InvariantCulture);
                    }

                    instanceClaim.SetValue(instance, dateTimeOffsetValue);
                }
                else if (instanceClaim.PropertyType == typeof(string))
                {
                    var stringValue = claim.Value;

                    instanceClaim.SetValue(instance, stringValue);
                }
                else
                {
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

            if (webApiClaimAttribute.DoNotListOnSubjectClaims)
            {
                continue;
            }

            claimType = webApiClaimAttribute.Type ?? instanceClaim.Name;

            var claimObjectValue = instanceClaim.GetValue(instance);

            if (claimObjectValue is null)
            {
                if (webApiClaimAttribute.Required)
                {
                    throw new InvalidOperationException($"Missing required claim value for {claimType}");
                }

                continue;
            }

            string? claimValue;

            if ((instanceClaim.PropertyType == typeof(DateTime)) || (instanceClaim.PropertyType == typeof(DateTime?)))
            {
                var dateTimeValue = (DateTime)claimObjectValue;

                claimValue = new DateTimeOffset(dateTimeValue).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            }
            else if ((instanceClaim.PropertyType == typeof(DateTimeOffset)) || (instanceClaim.PropertyType == typeof(DateTimeOffset?)))
            {
                var dateTimeOffsetValue = (DateTimeOffset)claimObjectValue;

                claimValue = dateTimeOffsetValue.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                claimValue = claimObjectValue.ToString();
            }

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
        IssuedAt = DateTimeOffset.UtcNow;

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
            ExpiresAt = TimeZoneInfo.ConvertTime(ExpiresAt.Value, TimeZoneInfo.Utc);

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
