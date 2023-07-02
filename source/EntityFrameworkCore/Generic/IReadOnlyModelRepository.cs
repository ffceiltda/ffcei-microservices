using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore.Generic;

/// <summary>
/// Generic Read-Only Model Repository interface
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IReadOnlyModelRepository<TModel> : IReadOnlyModelRepository where TModel : class, IModel
{
    /// <summary>
    /// Create queryable expression by predicate
    /// </summary>
    /// <param name="predicate">Predicate for match</param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> Where(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Create queryable expression by predicate
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Predicate for match</param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> WhereAdvanced(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Create queryable expression
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>A queryable instance</returns>
    IQueryable<TModel> WhereAll(bool ignoreQueryFilters = false);

    /// <summary>
    /// Create queryable expression ordered by
    /// </summary>
    /// <typeparam name="TKey">Result key type</typeparam>
    /// <param name="keySelector">Key selector</param>
    /// <returns>A queryable instance</returns>
    IOrderedQueryable<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> keySelector);

    /// <summary>
    /// Create queryable expression ordered by
    /// </summary>
    /// <typeparam name="TKey">Result key type</typeparam>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="keySelector">Key selector</param>
    /// <returns>A queryable instance</returns>
    IOrderedQueryable<TModel> OrderByAdvanced<TKey>(bool ignoreQueryFilters, Expression<Func<TModel, TKey>> keySelector);

    /// <summary>
    /// Create queryable expression ordered by descending
    /// </summary>
    /// <typeparam name="TKey">Result key type</typeparam>
    /// <param name="keySelector">Key selector</param>
    /// <returns>A queryable instance</returns>
    IOrderedQueryable<TModel> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> keySelector);

    /// <summary>
    /// Create queryable expression ordered by descending
    /// </summary>
    /// <typeparam name="TKey">Result key type</typeparam>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="keySelector">Key selector</param>
    /// <returns>A queryable instance</returns>
    IOrderedQueryable<TModel> OrderByDescendingAdvanced<TKey>(bool ignoreQueryFilters, Expression<Func<TModel, TKey>> keySelector);

    /// <summary>
    /// Return all models in repository, allowing to ignore EF Query Filters
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    IAsyncEnumerable<TModel> AllAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return all models in repository, allowing to ignore EF Query Filters, as a List
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    Task<List<TModel>> AllAsListAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return all models in repository, allowing to ignore EF Query Filters, as a HashSet
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    Task<HashSet<TModel>> AllAsHashSetAsync(bool ignoreQueryFilters = false);

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

    /// <summary>
    /// Return all models that match predicate or null, as a List
    /// </summary>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<List<TModel>> ManyByPredicateAsListAsync(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return all models that match predicate or null, allowing to ignore EF Query Filters, as a List
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<List<TModel>> ManyByPredicateAsListAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return all models that match predicate or null, as a HashSet
    /// </summary>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<HashSet<TModel>> ManyByPredicateAsHashSetAsync(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Return all models that match predicate or null, allowing to ignore EF Query Filters, as a HashSet
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>All models that match predicate or null</returns>
    Task<HashSet<TModel>> ManyByPredicateAsHashSetAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate);
}
