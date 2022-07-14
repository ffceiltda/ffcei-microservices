using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public abstract class ModelRepositoryContainer<T> : IModelRepositoryContainer where T : ModelRepositoryDbContext
    {
        public T Context { get; private set; }

        ModelRepositoryDbContext IModelRepositoryContainer.Context => Context;

        protected ModelRepositoryContainer(T context)
        {
            Context = context;
        }

        public int SaveChanges() => Context.SaveChanges();

        public int SaveChanges(bool acceptAllChangesOnSuccess) => Context.SaveChanges(acceptAllChangesOnSuccess);

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Context.SaveChangesAsync(cancellationToken);
    }
}
