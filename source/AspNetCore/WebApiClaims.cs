using System.Security.Claims;
using System.Globalization;

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
                    if (instanceClaim.PropertyType == typeof(Guid))
                    {
                        var guidValue = Guid.Parse(claim.Value);

                        instanceClaim.SetValue(instance, guidValue);
                    }
                    else if (instanceClaim.PropertyType == typeof(int))
                    {
                        var intValue = int.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, intValue);
                    }
                    else if (instanceClaim.PropertyType == typeof(long))
                    {
                        var longValue = long.Parse(claim.Value, CultureInfo.InvariantCulture);

                        instanceClaim.SetValue(instance, longValue);
                    }
                    else if (instanceClaim.PropertyType == typeof(bool))
                    {
                        var boolValue = bool.Parse(claim.Value);

                        instanceClaim.SetValue(instance, boolValue);
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
                            throw new InvalidOperationException($"Missing required claim attribute for {claimType}");
                        }

                        var webApiClaimAttribute = (attribute as WebApiClaimAttribute);

                        if (webApiClaimAttribute is null)
                        {
                            throw new InvalidOperationException($"Invalid required claim attribute for {claimType}");
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
    }
}
