using System.Security.Claims;
using System.Globalization;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api claims base class
    /// </summary>
    public class WebApiClaims
    {
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

        private static void DoParseClaims(object instance, ClaimsIdentity claims)
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        public void ParseClaims(ClaimsIdentity claims)
        {
            DoParseClaims(this, claims);
        }
    }
}
