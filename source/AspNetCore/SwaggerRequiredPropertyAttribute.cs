namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Mark property as required (present in a minimal request) by SwaggerDoc generator
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SwaggerRequiredPropertyAttribute : Attribute
{
}
