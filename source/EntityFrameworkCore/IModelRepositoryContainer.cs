namespace FFCEI.Microservices.EntityFrameworkCore
{
    public interface IModelRepositoryContainer
    {
        ModelRepositoryDbContext Context { get; }

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
