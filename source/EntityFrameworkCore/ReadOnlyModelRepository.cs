using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore
{
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

        /// <summary>
        /// Return queryable object
        /// </summary>
        /// <returns>Queryable object</returns>
        public IQueryable<TModel> Query() => Set;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        public async Task<IEnumerable<TModel>> AllAsync() => await Set.ToListAsync();

        public async Task<IEnumerable<IModel>> AllModelsAsync() => await AllAsync();

        public async ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys) => await Set.FindAsync(keys);

        public async ValueTask<TModel?> FirstOrDefaultByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Set.Where(predicate).FirstOrDefaultAsync();

        public async Task<IEnumerable<TModel>> ManyByPredicateAsync(Expression<Func<TModel, bool>> predicate) => await Set.Where(predicate).ToListAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
