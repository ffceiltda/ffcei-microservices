using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore.Generic;

/// <summary>
/// Generic Read-Only Model Repository interface
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IReadOnlyModelRepository<TModel> : IReadOnlyModelRepository where TModel : IModel
{
    /// <summary>
    /// Create queryable expression by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> Where(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Create queryable expression by predicate
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate"></param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> WhereAdvanced(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Create queryable expression
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> WhereAll(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return all models in repository, allowing to ignore EF Query Filters
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    Task<IEnumerable<TModel>> AllAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return first model that match predicate or null
    /// </summary>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>First model that match predicate or null</returns>
    ValueTask<TModel?> FirstOrDefaultByPredicateAsync(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return first model that match predicate or null, allowing to ignore EF Query Filters
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>First model that match predicate or null</returns>
    ValueTask<TModel?> FirstOrDefaultByPredicateAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return all models that match predicate or null
    /// </summary>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<IEnumerable<TModel>> ManyByPredicateAsync(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return all models that match predicate or null, allowing to ignore EF Query Filters
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<IEnumerable<TModel>> ManyByPredicateAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);
}
