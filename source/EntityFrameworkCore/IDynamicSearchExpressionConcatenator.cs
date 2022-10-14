namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Concatenator Between expressions interface
/// </summary>
public interface IDynamicSearchExpressionConcatenator
{
    /// <summary>
    /// Negate this concatenator (become AND NOT / OR NOT)
    /// </summary>
    bool Negate { get; set; }

    /// <summary>
    /// Dynamic Search Expression Bitwise Concatenator
    /// </summary>
    DynamicSearchExpressionBitwiseConcatenator Concatenator { get; set; }
}
