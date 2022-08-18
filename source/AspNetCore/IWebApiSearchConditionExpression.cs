namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Search Expression Condition
/// </summary>
public interface IWebApiSearchConditionExpression : IWebApiSearchExpression
{
    /// <summary>
    /// Negate this operator (become NOT equals, NOT Less, etc...)
    /// </summary>
    bool? Negate { get; set; }

    /// <summary>
    /// Web Api Search Expression Comparison Operator
    /// </summary>
    WebApiSearchExpressionComparisonOperator? ComparisonOperator { get; set; }

    /// <summary>
    /// One (or more) values involved in expression
    /// </summary>
    IReadOnlyList<string> Values { get; }
}
