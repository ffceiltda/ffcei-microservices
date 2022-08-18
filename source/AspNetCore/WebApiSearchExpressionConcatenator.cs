namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Search Expression Concatenator Between expressions
/// </summary>
public class WebApiSearchExpressionConcatenator
{
    /// <summary>
    /// Negate this concatenator (become AND NOT / OR NOT)
    /// </summary>
    public bool Negate { get; set; }

    /// <summary>
    /// Web Api Search Expression Bitwise Concatenator
    /// </summary>
    public WebApiSearchExpressionBitwiseConcatenator Concatenator { get; set; }
}
