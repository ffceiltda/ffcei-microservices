using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public class ReadOnlyModelRepository<TModel> : IReadOnlyModelRepository<TModel> where TModel : Model
    {
        protected ModelRepositoryDbContext Context { get; private set; }

        protected DbSet<TModel> Set => Context.Set<TModel>();

        public ReadOnlyModelRepository(ModelRepositoryDbContext context) => Context = context;

        public IQueryable<TModel> Query() => Set;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        public async Task<IReadOnlyList<TModel>> AllAsync() => await Set.ToListAsync();

        public async Task<IReadOnlyList<IModel>> AllModelsAsync() => await AllAsync();

        public async ValueTask<TModel?> FirstOrDefaultByKey(params object[] keys) => await Set.FindAsync(keys);

        public async ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys) => await FirstOrDefaultByKey(keys);

        public async ValueTask<TModel?> FirstOrDefaultByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Set.Where(predicate).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<TModel>> ManyByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Set.Where(predicate).ToListAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
