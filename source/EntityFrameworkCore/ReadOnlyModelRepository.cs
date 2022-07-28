using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Generic Read-Only Model Repository
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class ReadOnlyModelRepository<TModel> : IReadOnlyModelRepository<TModel> where TModel : Model
{
    protected ModelRepositoryDbContext Context { get; private set; }

    protected DbSet<TModel> Set => Context.Set<TModel>();

    /// <summary>
    /// Read-Only Model Repository constructor
    /// </summary>
    /// <param name="context">Model Repository DbContext instance</param>
    public ReadOnlyModelRepository(ModelRepositoryDbContext context) => Context = context;

    public IQueryable<TModel> Where(Expression<Func<TModel, bool>> predicate) => Set.Where(predicate);

    public IQueryable<TModel> WhereAdvanced(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate) => Set.IgnoreQueryFilters().Where(predicate);

    public IQueryable<TModel> WhereAll(bool ignoreQueryFilters = false) => ignoreQueryFilters ? Set.IgnoreQueryFilters() : Set;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
    public async Task<IEnumerable<TModel>> AllAsync(bool ignoreQueryFilters = false) => await WhereAll(ignoreQueryFilters).ToListAsync();

    public async Task<IEnumerable<IModel>> AllModelsAsync(bool ignoreQueryFilters = false) => await AllAsync(ignoreQueryFilters);

    public async ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys) => await Set.FindAsync(keys);

    public async ValueTask<IModel?> FirstOrDefaultByKeyAdvancedAsync(bool ignoreQueryFilters, params object[] keys)
    {
        var result = await Set.FindAsync(keys);

        if (ignoreQueryFilters && (result is ILogicallyDeletableModel logicallyDeletedResult))
        {
            if (logicallyDeletedResult.IsLogicallyDeleted)
            {
                return null;
            }
        }

        return result;
    }

    public async ValueTask<TModel?> FirstOrDefaultByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Where(predicate).FirstOrDefaultAsync();

    public async ValueTask<TModel?> FirstOrDefaultByPredicateAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate) =>
        await WhereAdvanced(ignoreQueryFilters, predicate).FirstOrDefaultAsync();

    public async Task<IEnumerable<TModel>> ManyByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Where(predicate).ToListAsync();

    public async Task<IEnumerable<TModel>> ManyByPredicateAdvancedAsync(bool ignoreQueryFilters, Expression<Func<TModel, bool>> predicate) =>
         await WhereAdvanced(ignoreQueryFilters, predicate).ToListAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
}
