namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Concatenator Between expressions
/// </summary>
public class DynamicSearchExpressionConcatenator : IDynamicSearchExpressionConcatenator
{
    public bool Negate { get; set; }

    public DynamicSearchExpressionBitwiseConcatenator Concatenator { get; set; }
}
