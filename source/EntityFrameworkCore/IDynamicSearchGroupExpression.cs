namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Group interface
/// </summary>
public interface IDynamicSearchGroupExpression : IDynamicSearchExpression
{
    /// <summary>
    /// List of expressions
    /// </summary>
    IReadOnlyList<IDynamicSearchExpression> Expressions { get; }
}
