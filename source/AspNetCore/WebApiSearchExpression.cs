using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Search Expression
/// </summary>
public class WebApiSearchExpression : IWebApiSearchGroupExpression, IWebApiSearchConditionExpression
{
    public WebApiSearchExpressionConcatenator? Concatenator { get; set; }

    [JsonIgnore]
    IReadOnlyList<IWebApiSearchExpression> IWebApiSearchGroupExpression.Expressions => Expressions;
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public List<WebApiSearchExpression> Expressions { get; set; } = new();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

    public bool? Negate { get; set; }
    public WebApiSearchExpressionComparisonOperator? ComparisonOperator { get; set; }
    [JsonIgnore]
    IReadOnlyList<string> IWebApiSearchConditionExpression.Values => Values;
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> Values { get; set; } = new();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists
}
