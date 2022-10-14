namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Bitwise Concatenator (AND / OR) between expressions
/// </summary>
public enum DynamicSearchExpressionBitwiseConcatenator : int
{
    /// <summary>
    /// AND bitwise expression concatenator
    /// </summary>
    And = 0,
    /// <summary>
    /// OR bitwise expression concatenator
    /// </summary>
    Or = 1
}
