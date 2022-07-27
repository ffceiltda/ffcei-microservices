namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Mark message to be processed by SwaggerDoc generator
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class SwaggerRequestAttribute : Attribute
{
}
