namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api Claim attibute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class WebApiClaimAttribute : Attribute
    {
        public bool Required { get; set; } = true;
        public string Type { get; set; } = null!;
    }
}
