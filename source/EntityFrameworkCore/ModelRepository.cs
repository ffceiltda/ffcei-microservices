using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Generic Model Repository
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class ModelRepository<TModel> : ReadOnlyModelRepository<TModel>, IModelRepository<TModel> where TModel : Model
{
    /// <summary>
    /// Model Repository constructor
    /// </summary>
    /// <param name="context">Model Repository DbContext instance</param>
    public ModelRepository(ModelRepositoryDbContext context) : base(context) { }

#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
    public async Task AddNewAsync(TModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        await Set.AddAsync(content);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task AddNewAsync(IModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel casted)
        {
            await AddNewAsync(casted, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task AddManyAsync(IEnumerable<TModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        await Set.AddRangeAsync(contents);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task AddManyAsync(IEnumerable<IModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        foreach (var content in contents)
        {
            if (content is not TModel casted)
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
            }
        }

        foreach (var content in contents)
        {
            await AddNewAsync((TModel)content, false);
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task UpdateExistingAsync(TModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        Set.Update(content);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task UpdateExistingAsync(IModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel casted)
        {
            await UpdateExistingAsync(casted, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task RemoveExistingAsync(TModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        Set.Remove(content);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task RemoveExistingAsync(IModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel casted)
        {
            await RemoveExistingAsync(casted, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task RemoveManyAsync(IEnumerable<TModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        Set.RemoveRange(contents);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task RemoveManyAsync(IEnumerable<IModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        if (contents is IEnumerable<TModel> casted)
        {
            await RemoveManyAsync(casted, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(IEnumerable<TModel>), contents.GetType());
        }
    }

    public async Task RemoveByKeyAsync(bool autoCommit = true, params object[] keys)
    {
        if (keys is null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        var content = await FirstOrDefaultByKeyAsync(keys);

        if (content is not null)
        {
            await RemoveExistingAsync(content, autoCommit);
        }
    }

    public async Task RemoveManyByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true)
    {
        var contents = await Set.Where(predicate).ToListAsync();

        if (contents is null)
        {
            throw new ArgumentNullException(nameof(predicate), "predicate returned null");
        }

        foreach (var content in contents)
        {
            await RemoveExistingAsync(content, false);
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task ReloadExistingAsync(TModel content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        await Context.Entry(content).ReloadAsync();
    }

    public async Task ReloadExistingAsync(IModel content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel casted)
        {
            await ReloadExistingAsync(casted);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning restore IDE0058 // Expression value is never used

    public void Detach(TModel content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        Context.Entry(content).State = EntityState.Detached;
    }

    public void Detach(IModel content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel casted)
        {
            Detach(casted);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }
}
