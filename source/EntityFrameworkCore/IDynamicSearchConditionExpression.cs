namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Condition interface
/// </summary>
public interface IDynamicSearchConditionExpression : IDynamicSearchExpression
{
    /// <summary>
    /// Negate this operator (become NOT equals, NOT Less, etc...)
    /// </summary>
    bool? Negate { get; set; }

    /// <summary>
    /// Dynamic Search Expression Comparison Condition
    /// </summary>
    string? ComparisionCondition { get; set; }

    /// <summary>
    /// Dynamic Search Expression Comparison Operator
    /// </summary>
    DynamicSearchExpressionComparisonOperator? ComparisonOperator { get; set; }

    /// <summary>
    /// One (or more) values involved in expression
    /// </summary>
    IReadOnlyList<string> Values { get; }
}
