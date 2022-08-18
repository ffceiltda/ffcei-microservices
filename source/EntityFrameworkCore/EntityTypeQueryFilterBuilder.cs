using FFCEI.Microservices.Models;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Composite query filter support in EF Core model builder
/// </summary>
/// <typeparam name="TEntity">Model entity type</typeparam>
public class EntityTypeQueryFilterBuilder<TEntity> : IEntityTypeQueryFilterBuilder<TEntity> where TEntity : class, IModel
{
    private class QueryFilterExpressionBuilderReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public QueryFilterExpressionBuilderReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node) => ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
    }

    private class QueryFilterExpression
    {
        public string? Name { get; set; }
        public Expression<Func<TEntity, bool>>? Expression { get; set; }
        public bool IsActive { get; set; }

        public QueryFilterExpression(string? name, Expression<Func<TEntity, bool>>? expression, bool isActive)
        {
            Name = name;
            Expression = expression;
            IsActive = isActive;
        }
    }

    private readonly Dictionary<string, QueryFilterExpression> _queryFilterExpressions = new();

    /// <summary>
    /// Default constructor
    /// </summary>
    public EntityTypeQueryFilterBuilder()
    {
    }

    public IEntityTypeQueryFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> expression, bool active = true)
    {
        string key = Guid.NewGuid().ToString();

        return AddFilter(key.ToString(), expression, active);
    }

    public IEntityTypeQueryFilterBuilder<TEntity> AddFilter(string filterName, Expression<Func<TEntity, bool>> expression, bool active = true)
    {
        _queryFilterExpressions.Add(filterName, new QueryFilterExpression(filterName, expression, active));

        return this;
    }

    public IEntityTypeQueryFilterBuilder<TEntity> EnableFilter(string filterName)
    {
        var queryFilter = _queryFilterExpressions[filterName];

        queryFilter.IsActive = true;

        return this;
    }

    public IEntityTypeQueryFilterBuilder<TEntity> DisableFilter(string filterName)
    {
        var queryFilter = _queryFilterExpressions[filterName];

        queryFilter.IsActive = false;

        return this;
    }

    public bool HasFilter(string filterName) => _queryFilterExpressions.ContainsKey(filterName);

    public bool IsFilterActive(string filterName) => HasFilter(filterName) && _queryFilterExpressions[filterName].IsActive;

    public bool IsEmpty => _queryFilterExpressions.Count == 0;

    public bool IsNoOp => !_queryFilterExpressions.Any(expression => expression.Value.IsActive);

    public Expression<Func<TEntity, bool>> Build()
    {
        if (_queryFilterExpressions.Count == 0)
        {
            throw new InvalidOperationException("No expressions provided.");
        }

        var activeQueryFilters = _queryFilterExpressions.Where(q => q.Value.IsActive).Select(q => q.Value.Expression).ToList();

        if (activeQueryFilters.Count == 0)
        {
            return q => true;
        }

        var expression = activeQueryFilters.First();

        if (expression is null)
        {
            throw new InvalidOperationException($"{nameof(expression)} is null");
        }

        if (activeQueryFilters.Count == 1)
        {
            return expression;
        }

        foreach (var nextExpression in activeQueryFilters.Skip(1))
        {
            if (nextExpression is null)
            {
                throw new InvalidOperationException($"{nameof(nextExpression)} is null");
            }

            var leftParameter = expression.Parameters[0];
            var rightParameter = nextExpression.Parameters[0];

            var visitor = new QueryFilterExpressionBuilderReplaceParameterVisitor(rightParameter, leftParameter);

            var leftBody = expression.Body;
            var rightBody = visitor.Visit(nextExpression.Body);

            expression = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(leftBody, rightBody), leftParameter);
        }

        return expression;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString()
    {
        return base.ToString();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
