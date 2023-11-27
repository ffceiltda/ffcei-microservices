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
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        await Set.AddAsync(model);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task AddNewAsync(IModel content, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        if (content is TModel model)
        {
            await AddNewAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task AddManyAsync(ICollection<TModel> models, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        await Set.AddRangeAsync(models);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task AddManyAsync(ICollection<IModel> contents, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(contents, nameof(contents));

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
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        Set.Update(model);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task UpdateExistingAsync(IModel content, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        if (content is TModel model)
        {
            await UpdateExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task UpdateManyAsync(ICollection<TModel> models, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        Set.UpdateRange(models);

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task UpdateManyAsync(ICollection<IModel> contents, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(contents, nameof(contents));

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

    public async Task<IEnumerable<TModel>> LogicallyDeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true)
    {
        var models = await WhereAdvanced(false, predicate).ToListAsync();

        if (models is null)
        {
            throw new ArgumentNullException(nameof(predicate), "predicate returned null");
        }

        await LogicallyDeleteManyAsync(models, autoCommit);

        return models;
    }

    public async Task LogicallyDeleteExistingAsync(TModel model, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        if (model is ILogicallyDeletableModel logicallyDeletableModel)
        {
            logicallyDeletableModel.LogicallyDelete();

            Set.Update(model);
        }
        else
        {
            throw new InvalidCastException("model does not implement interface ILogicallyDeletableModel");
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async ValueTask<IModel?> LogicallyDeleteByKeyAsync(bool autoCommit, params object[] keys)
    {
        var model = await FirstOrDefaultByKeyAdvancedAsync(false, keys);

        if (model is not null)
        {
            await LogicallyDeleteExistingAsync(model, autoCommit);
        }

        return model;
    }

    public async Task LogicallyDeleteExistingAsync(IModel content, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        if (content is TModel model)
        {
            await LogicallyDeleteExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task LogicallyDeleteManyAsync(ICollection<TModel> models, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        var modelType = models.FirstOrDefault();

        if (modelType is ILogicallyDeletableModel)
        {
            var index = 0;

            foreach (var model in models)
            {
                if (model is ILogicallyDeletableModel logicallyDeletableModel)
                {
                    logicallyDeletableModel.LogicallyDelete();
                }
                else
                {
                    throw new InvalidCastException($"models[{index}] does not implement interface ILogicallyDeletableModel");
                }

                ++index;
            }

            Set.UpdateRange(models);
        }
        else if (modelType is not null)
        {
            throw new InvalidCastException($"models does not implement interface ILogicallyDeletableModel");
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task LogicallyDeleteManyAsync(ICollection<IModel> contents, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(contents, nameof(contents));

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

    public async Task<IEnumerable<TModel>> LogicallyUndeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true)
    {
        var models = await WhereAdvanced(true, predicate).ToListAsync();

        if (models is null)
        {
            throw new ArgumentNullException(nameof(predicate), "predicate returned null");
        }

        await LogicallyUndeleteManyAsync(models, autoCommit);

        return models;
    }

    public async Task LogicallyUndeleteExistingAsync(TModel model, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        if (model is ILogicallyDeletableModel logicallyDeletableModel)
        {
            logicallyDeletableModel.LogicallyUndelete();

            Set.Update(model);
        }
        else
        {
            throw new InvalidCastException("model does not implement interface ILogicallyDeletableModel");
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async ValueTask<IModel?> LogicallyUndeleteByKeyAsync(bool autoCommit, params object[] keys)
    {
        var model = await FirstOrDefaultByKeyAdvancedAsync(true, keys);

        if (model is not null)
        {
            await LogicallyUndeleteExistingAsync(model, autoCommit);
        }

        return model;
    }

    public async Task LogicallyUndeleteExistingAsync(IModel content, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        if (content is TModel model)
        {
            await LogicallyUndeleteExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task LogicallyUndeleteManyAsync(ICollection<TModel> models, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        var modelType = models.FirstOrDefault();

        if (modelType is ILogicallyDeletableModel)
        {
            var index = 0;

            foreach (var model in models)
            {
                if (model is ILogicallyDeletableModel logicallyDeletableModel)
                {
                    logicallyDeletableModel.LogicallyUndelete();
                }
                else
                {
                    throw new InvalidCastException($"models[{index}] does not implement interface ILogicallyDeletableModel");
                }

                ++index;
            }

            Set.UpdateRange(models);
        }
        else if (modelType is not null)
        {
            throw new InvalidCastException($"models does not implement interface ILogicallyDeletableModel");
        }

        if (autoCommit)
        {
            await Context.SaveChangesAsync();
        }
    }

    public async Task LogicallyUndeleteManyAsync(ICollection<IModel> contents, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(contents, nameof(contents));

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

        await LogicallyUndeleteManyAsync(models, autoCommit);
    }

    public async Task RemoveExistingAsync(TModel model, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

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
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        if (content is TModel model)
        {
            await RemoveExistingAsync(model, autoCommit);
        }
        else
        {
            throw new IncompatibleModelObjectTypeForDbSetException(typeof(TModel), content.GetType());
        }
    }

    public async Task RemoveManyAsync(ICollection<TModel> models, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

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

    public async Task RemoveManyAsync(ICollection<IModel> contents, bool autoCommit = true)
    {
        ArgumentNullException.ThrowIfNull(contents, nameof(contents));

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
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var content = await FirstOrDefaultByKeyAsync(keys);

        if (content is not null)
        {
            await RemoveExistingAsync(content, autoCommit);
        }
    }

    public async Task RemoveManyByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true)
    {
        var models = await Where(predicate).ToListAsync();

        if (models is null)
        {
            throw new ArgumentNullException(nameof(predicate), "predicate returned null");
        }

        await RemoveManyAsync(models, autoCommit);
    }

    public async Task ReloadExistingAsync(TModel content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        await Context.Entry(content).ReloadAsync();
    }

    public async Task ReloadExistingAsync(IModel content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

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
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        Context.Entry(content).State = EntityState.Detached;
    }

    public void Detach(IModel content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

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
