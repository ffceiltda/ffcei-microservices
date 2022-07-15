namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Generic Model Repository Container abstract class
    /// </summary>
    public abstract class ModelRepositoryContainer<TDbContext> : IModelRepositoryContainer where TDbContext : ModelRepositoryDbContext
    {
        /// <summary>
        /// Associated ModelRepositoryDbContext (of type TDbContext)
        /// </summary>
        public TDbContext Context { get; private set; }

        ModelRepositoryDbContext IModelRepositoryContainer.Context => Context;

        protected ModelRepositoryContainer(TDbContext context)
        {
            Context = context;
        }

        public int SaveChanges() => Context.SaveChanges();

        public int SaveChanges(bool acceptAllChangesOnSuccess) => Context.SaveChanges(acceptAllChangesOnSuccess);

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Context.SaveChangesAsync(cancellationToken);
    }
}
