namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Dynamic Search Expression Comparison Operator in expressions
/// </summary>
public enum DynamicSearchExpressionComparisonOperator : int
{
    /// <summary>
    /// Equals comparison operator
    /// </summary>
    Equal = 0,
    /// <summary>
    /// Less Than comparsion operator
    /// </summary>
    Less = 1,
    /// <summary>
    /// Less (or Equals) Than comparison operator
    /// </summary>
    LessOrEqual = 2,
    /// <summary>
    /// Greater Than comparison operator
    /// </summary>
    Greater = 3,
    /// <summary>
    /// Greater (or Equals) Than comparison operator
    /// </summary>
    GreaterOrEqual = 4,
    /// <summary>
    /// Between comparison operator
    /// </summary>
    Between = 5,
    /// <summary>
    /// Contains Value comparison operator
    /// </summary>
    Contains = 6,
    /// <summary>
    /// Has Substring Value comparison operator
    /// </summary>
    HasSubstring = 7,
    /// <summary>
    /// Value is NULL
    /// </summary>
    IsNull = 8
}
