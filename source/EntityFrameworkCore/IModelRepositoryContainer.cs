namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Model Repository Container interface
    /// </summary>
    public interface IModelRepositoryContainer
    {
        /// <summary>
        /// Associated ModelRepositoryDbContext
        /// </summary>
        ModelRepositoryDbContext Context { get; }

        /// <summary>
        /// See DbContext.SaveChanges
        /// </summary>
        /// <returns>Rows affected</returns>
        int SaveChanges();

        /// <summary>
        /// See DbContext.SaveChanges
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">See DbContext.SaveChanges</param>
        /// <returns>Rows affected</returns>
        int SaveChanges(bool acceptAllChangesOnSuccess);

        /// <summary>
        /// See DbContext.SaveChangesAsync
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">See DbContext.SaveChangesAsync</param>
        /// <param name="cancellationToken">See DbContext.SaveChangesAsync</param>
        /// <returns>Rows affected</returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        /// <summary>
        /// See DbContext.SaveChangesAsync
        /// </summary>
        /// <param name="cancellationToken">See DbContext.SaveChangesAsync</param>
        /// <returns>Rows affected</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
