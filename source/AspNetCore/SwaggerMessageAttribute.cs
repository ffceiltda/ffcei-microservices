namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Mark message to be processed by SwaggerDoc generator
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SwaggerMessageAttribute : Attribute
    {
    }
}
