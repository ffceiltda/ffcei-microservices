using System.Text.Json.Serialization;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression
/// </summary>
public class DynamicSearchExpression : IDynamicSearchGroupExpression, IDynamicSearchConditionExpression
{
    public IDynamicSearchExpressionConcatenator? Concatenator { get; set; }

    public bool? Negate { get; set; }

    [JsonIgnore]
    IReadOnlyList<IDynamicSearchExpression> IDynamicSearchGroupExpression.Expressions => Expressions;

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public List<DynamicSearchExpression> Expressions { get; set; } = new();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

    public string? ComparisionCondition { get; set; }

    public DynamicSearchExpressionComparisonOperator? ComparisonOperator { get; set; }

    [JsonIgnore]
    IReadOnlyList<string> IDynamicSearchConditionExpression.Values => Values;
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

    public List<string> Values { get; set; } = new();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists
}
