namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Search Expression base interface
/// </summary>
public interface IWebApiSearchExpression
{
    /// <summary>
    /// Web Api Search Expression Concatenator 
    /// </summary>
    WebApiSearchExpressionConcatenator? Concatenator { get; set; }
}
