using FFCEI.Microservices.EntityFrameworkCore;
using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Generic Read-Only Model Repository interface extension methods for IIdAwareModel repositories
/// </summary>
public static class IReadOnlyModelRepositoryITimestampedModelExtensionMethods
{
    /// <summary>
    /// Return last model or null, ordered by Id column
    /// </summary>
    /// <param name="repository">Model repository</param>>
    /// <returns>Last model that match predicate or null</returns>
    public static async ValueTask<TModel?> LastOrDefaultByPredicateAsync<TModel>(this IReadOnlyModelRepository<TModel> repository)
        where TModel : class, ITimestampedModel
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        return await repository.Any.OrderBy(r => r.CreatedAt).LastOrDefaultAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }

    /// <summary>
    /// Return last model that match predicate or null, ordered by Id column
    /// </summary>
    /// <param name="repository">Model repository</param>>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>Last model that match predicate or null</returns>
    public static async ValueTask<TModel?> LastOrDefaultByPredicateAsync<TModel>(this IReadOnlyModelRepository<TModel> repository, Expression<Func<TModel, bool>> predicate)
        where TModel : class, ITimestampedModel
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        return await repository.Where(predicate).OrderBy(r => r.CreatedAt).LastOrDefaultAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }

    /// <summary>
    /// Return last model or null, allowing to ignore EF Query Filters, ordered by Id column
    /// </summary>
    /// <param name="repository">Model repository</param>>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <returns>Last model that match predicate or null</returns>
    public static async ValueTask<TModel?> LastOrDefaultByPredicateAdvancedAsync<TModel>(this IReadOnlyModelRepository<TModel> repository, bool ignoreQueryFilters)
        where TModel : class, ITimestampedModel
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        return await repository.AnyAdvanced(ignoreQueryFilters).OrderBy(r => r.CreatedAt).LastOrDefaultAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }

    /// <summary>
    /// Return last model that match predicate or null, allowing to ignore EF Query Filters, ordered by Id column
    /// </summary>
    /// <param name="repository">Model repository</param>>
    /// <param name="ignoreQueryFilters">Ignore EF Core Query Filters</param>
    /// <param name="predicate">Model match predicate</param>
    /// <returns>Last model that match predicate or null</returns>
    public static async ValueTask<TModel?> LastOrDefaultByPredicateAdvancedAsync<TModel>(this IReadOnlyModelRepository<TModel> repository, bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate)
        where TModel : class, ITimestampedModel
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        return await repository.WhereAdvanced(ignoreQueryFilters, predicate).OrderBy(r => r.CreatedAt).LastOrDefaultAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
