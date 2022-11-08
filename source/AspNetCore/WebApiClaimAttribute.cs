namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Claim attibute
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class WebApiClaimAttribute : Attribute
{
    /// <summary>
    /// Indicates if this attribute is required
    /// </summary>
    public bool Required { get; set; } = true;

    /// <summary>
    /// The claim type (identifier) string
    /// </summary>
    public string Type { get; set; } = null!;

    public bool DoNotListOnSubjectClaims { get; set; }
}
