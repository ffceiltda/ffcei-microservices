namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression base interface
/// </summary>
public interface IDynamicSearchExpression
{
    /// <summary>
    /// Dynamic Search Expression Concatenator 
    /// </summary>
    IDynamicSearchExpressionConcatenator? Concatenator { get; set; }
}
