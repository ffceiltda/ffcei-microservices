namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Search Expression Group
/// </summary>
public interface IWebApiSearchGroupExpression : IWebApiSearchExpression
{
    /// <summary>
    /// List of expressions
    /// </summary>
    IReadOnlyList<IWebApiSearchExpression> Expressions { get; }
}
