using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Read-Only Model Repository
/// </summary>
public interface IReadOnlyModelRepository
{
    /// <summary>
    /// Create queryable expression
    /// </summary>
    /// <returns>A queryable instance</returns>
    IQueryable<IModel> AnyModel { get; }

    /// <summary>
    /// Return all models in repository
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    IAsyncEnumerable<IModel> AllModelsAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return all models in repository, as a List
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    Task<List<IModel>> AllModelsAsListAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return all models in repository, as a HashSet
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>All models in repository</returns>
    Task<HashSet<IModel>> AllModelsAsHashSetAsync(bool ignoreQueryFilters = false);

    /// <summary>
    /// Return first model that match keys or null
    /// </summary>
    /// <param name="keys">Model lookup keys</param>
    /// <returns>First model that match keys or null</returns>
    ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys);

    /// <summary>
    /// Return first model that match keys or null
    /// </summary>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="keys">Model lookup keys</param>
    /// <returns>First model that match keys or null</returns>
    ValueTask<IModel?> FirstOrDefaultByKeyAdvancedAsync(bool ignoreQueryFilters, params object[] keys);
}
