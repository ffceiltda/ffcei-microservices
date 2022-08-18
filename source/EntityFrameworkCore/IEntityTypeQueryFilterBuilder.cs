using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Interface for composite query filter support in EF Core model builder
/// </summary>
public interface IEntityTypeQueryFilterBuilder
{
    /// <summary>
    /// Searching for a filter based on the given name
    /// </summary>
    /// <param name="filterName">The unique name of the filter.</param>
    /// <returns>True if found, false otherwise</returns>
    bool HasFilter(string filterName);

    /// <summary>
    /// Searching for a filter based on the given name, and check if it's active
    /// </summary>
    /// <param name="filterName">The unique name of the filter.</param>
    /// <returns>True if found and active, false otherwise</returns>
    bool IsFilterActive(string filterName);

    /// <summary>
    /// Check if filter expression list is empty
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Check if filter expression list is a no-op (no active expressions)
    /// </summary>
    bool IsNoOp { get; }
}

/// <summary>
/// Interface for composite query filter support in EF Core model builder
/// </summary>
/// <typeparam name="TEntity">Model entity type</typeparam>
public interface IEntityTypeQueryFilterBuilder<TEntity> : IEntityTypeQueryFilterBuilder where TEntity : class, IModel
{
    /// <summary>
    /// Adding a filter to a given query filter builder.
    /// </summary>
    /// <param name="expression">A LINQ predicate expression.</param>
    /// <param name="active">Indication of whether the filter should be applied, this parameter can be controlled by a service injected into DbContext.</param>
    /// <returns>A IEntityTypeQueryFilterBuilder&lt;TEntity&gt; instance to chain methods.</returns>
    IEntityTypeQueryFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> expression, bool active = true);

    /// <summary>
    /// Adding a filter to a given query filter builder.
    /// </summary>
    /// <param name="filterName">The unique name of the filter.</param>
    /// <param name="expression">A LINQ predicate expression.</param>
    /// <param name="active">Indication of whether the filter should be applied, this parameter can be controlled by a service injected into DbContext.</param>
    /// <exception cref="ArgumentException">Filter with given name already exists.</exception>
    /// <exception cref="ArgumentNullException">Filter name is null.</exception>
    /// <returns>A IEntityTypeQueryFilterBuilder&lt;TEntity&gt; instance to chain methods.</returns>
    IEntityTypeQueryFilterBuilder<TEntity> AddFilter(string filterName, Expression<Func<TEntity, bool>> expression, bool active = true);

    /// <summary>
    /// Searching for a filter based on the given name and enables it.
    /// </summary>
    /// <param name="filterName">The unique name of the filter.</param>
    /// <exception cref="KeyNotFoundException">Filter with given name does not exist.</exception>
    /// <exception cref="ArgumentNullException">Filter name is null.</exception>
    /// <returns>A IEntityTypeQueryFilterBuilder&lt;TEntity&gt; instance to chain methods.></returns>
    IEntityTypeQueryFilterBuilder<TEntity> EnableFilter(string filterName);

    /// <summary>
    /// Searching for a filter based on the given name and disables it.
    /// </summary>
    /// <param name="filterName">The unique name of the filter.</param>
    /// <exception cref="KeyNotFoundException">Filter with given name does not exist.</exception>
    /// <exception cref="ArgumentNullException">Filter name is null.</exception>
    /// <returns>A IEntityTypeQueryFilterBuilder&lt;TEntity&gt; instance to chain methods.</returns>
    IEntityTypeQueryFilterBuilder<TEntity> DisableFilter(string filterName);

    /// <summary>
    /// Combine all active expressions into one expression. 
    /// </summary>
    /// <exception cref="InvalidOperationException">The builder contains no filters.</exception>
    /// <returns>A LINQ predicate expression.</returns>
    Expression<Func<TEntity, bool>> Build();
}
