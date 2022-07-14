using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public class ModelRepository<TModel> : ReadOnlyModelRepository<TModel>, IModelRepository<TModel> where TModel : Model
    {
        public ModelRepository(ModelRepositoryDbContext context) : base(context) { }

#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        public async Task AddNewAsync(TModel content, bool autoCommit = true)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await Set.AddAsync(content);

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task AddNewAsync(IModel content, bool autoCommit = true)
        {
            if (content == null)
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

        public async Task AddNewRangeAsync(IEnumerable<TModel> contents, bool autoCommit = true)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            await Set.AddRangeAsync(contents);

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task AddNewRangeAsync(IEnumerable<IModel> contents, bool autoCommit = true)
        {
            if (contents == null)
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
                await SaveChangesAsync();
            }
        }

        public async Task UpdateExistingAsync(TModel content, bool autoCommit = true)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Set.Update(content);

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task UpdateExistingAsync(IModel content, bool autoCommit = true)
        {
            if (content == null)
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
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Set.Remove(content);

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task RemoveExistingAsync(IModel content, bool autoCommit = true)
        {
            if (content == null)
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

        public async Task RemoveMultiple(IList<TModel> content, bool autoCommit = true)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Set.RemoveRange(content);

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task RemoveMultiple(IList<IModel> content, bool autoCommit = true)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (content is IList<TModel> casted)
            {
                await RemoveMultiple(casted, autoCommit);
            }
            else
            {
                throw new IncompatibleModelObjectTypeForDbSetException(typeof(IList<TModel>), content.GetType());
            }
        }

        public async Task RemoveByKeyAsync(bool autoCommit = true, params object[] keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var content = await FirstOrDefaultByKey(keys);

            if (content is not null)
            {
                await RemoveExistingAsync(content, autoCommit);
            }
        }

        public async Task RemoveManyByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true)
        {
            var contents = await Set.Where(predicate).ToListAsync();

            if (contents == null)
            {
                throw new ArgumentNullException(nameof(predicate), "predicate returned null");
            }

            foreach (var content in contents)
            {
                await RemoveExistingAsync(content, false);
            }

            if (autoCommit)
            {
                await SaveChangesAsync();
            }
        }

        public async Task ReloadExistingAsync(TModel content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await Context.Entry(content).ReloadAsync();
        }

        public async Task ReloadExistingAsync(IModel content)
        {
            if (content == null)
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
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Context.Entry(content).State = EntityState.Detached;
        }

        public void Detach(IModel content)
        {
            if (content == null)
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

        public int SaveChanges() => Context.SaveChanges();

        public int SaveChanges(bool acceptAllChangesOnSuccess) => Context.SaveChanges(acceptAllChangesOnSuccess);

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Context.SaveChangesAsync(cancellationToken);
    }
}
