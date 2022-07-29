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
    public async Task AddNewAsync(TModel model, bool autoCommit = true)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        await Set.AddAsync(model);

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

        if (content is TModel model)
        {
            await AddNewAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task AddManyAsync(IEnumerable<TModel> models, bool autoCommit = true)
    {
        if (models is null)
        {
            throw new ArgumentNullException(nameof(models));
        }

        await Set.AddRangeAsync(models);

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

        var models = new List<TModel>();

        foreach (var content in contents)
        {
            if (content is TModel model)
            {
                models.Add(model);
            }
            else
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
            }
        }

        await AddManyAsync(models, autoCommit);
    }

    public async Task UpdateExistingAsync(TModel model, bool autoCommit = true)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        Set.Update(model);

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

        if (content is TModel model)
        {
            await UpdateExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task UpdateManyAsync(IEnumerable<TModel> models, bool autoCommit = true)
    {
        if (models is null)
        {
            throw new ArgumentNullException(nameof(models));
        }

        Set.UpdateRange(models);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task UpdateManyAsync(IEnumerable<IModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        var models = new List<TModel>();

        foreach (var content in contents)
        {
            if (content is TModel model)
            {
                models.Add(model);
            }
            else
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
            }
        }

        await UpdateManyAsync(models, autoCommit);
    }

    public async Task LogicallyDeleteExistingAsync(TModel model, bool autoCommit = true)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (model is ILogicallyDeletableModel logicallyDeletableModel)
        {
            logicallyDeletableModel.LogicallyDelete();

            Set.Update(model);
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task LogicallyDeleteExistingAsync(IModel content, bool autoCommit = true)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (content is TModel model)
        {
            await LogicallyDeleteExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task LogicallyDeleteManyAsync(IEnumerable<TModel> models, bool autoCommit = true)
    {
        if (models is null)
        {
            throw new ArgumentNullException(nameof(models));
        }

        var modelType = models.FirstOrDefault();

        if (modelType is ILogicallyDeletableModel)
        {
            foreach (var model in models)
            {
                if (model is ILogicallyDeletableModel logicallyDeletableModel)
                {
                    logicallyDeletableModel.LogicallyDelete();
                }
            }

            Set.UpdateRange(models);
        }
        else
        {
            Set.RemoveRange(models);
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task LogicallyDeleteManyAsync(IEnumerable<IModel> contents, bool autoCommit = true)
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        var models = new List<TModel>();

        foreach (var content in contents)
        {
            if (content is TModel model)
            {
                models.Add(model);
            }
            else
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
            }
        }

        await LogicallyDeleteManyAsync(models, autoCommit);
    }

    public async Task RemoveExistingAsync(TModel model, bool autoCommit = true)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (model is ILogicallyDeletableModel)
        {
            await LogicallyDeleteExistingAsync(model, false);
        }
        else
        {
            Set.Remove(model);
        }

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

        if (content is TModel model)
        {
            await RemoveExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task RemoveManyAsync(IEnumerable<TModel> models, bool autoCommit = true)
    {
        if (models is null)
        {
            throw new ArgumentNullException(nameof(models));
        }

        var modelType = models.FirstOrDefault();

        if (modelType is ILogicallyDeletableModel)
        {
            await LogicallyDeleteManyAsync(models, false);
        }
        else
        {
            Set.RemoveRange(models);
        }

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

        var models = new List<TModel>();

        foreach (var content in contents)
        {
            if (content is TModel model)
            {
                models.Add(model);
            }
            else
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
            }
        }

        await RemoveManyAsync(models, autoCommit);
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
        var models = await Set.Where(predicate).ToListAsync();

        if (models is null)
        {
            throw new ArgumentNullException(nameof(predicate), "predicate returned null");
        }

        await RemoveManyAsync(models, autoCommit);
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

        if (content is TModel model)
        {
            await ReloadExistingAsync(model);
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

        if (content is TModel model)
        {
            Detach(model);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }
}
