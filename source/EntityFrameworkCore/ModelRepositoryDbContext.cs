using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// DbContext with support for Model objects
    /// </summary>
    public abstract class ModelRepositoryDbContext : DbContext
    {
        private static readonly PropertyInfo? CreatedAtProperty = typeof(TimeStampedModel).GetProperty("CreatedAt");
        private static readonly PropertyInfo? UpdatedAtProperty = typeof(TimeStampedModel).GetProperty("UpdatedAt");
        private static readonly PropertyInfo? UuidEnabledTimeStampedProperty = typeof(UuidAwareEnabledAwareTimeStampedModel).GetProperty("Uuid");
        private static readonly PropertyInfo? UuidTimeStampedProperty = typeof(UuidAwareTimeStampedModel).GetProperty("Uuid");
        private static readonly PropertyInfo? UuidEnabledProperty = typeof(UuidAwareEnabledAwareModel).GetProperty("Uuid");
        private static readonly PropertyInfo? UuidProperty = typeof(UuidAwareModel).GetProperty("Uuid");

        private bool _modelBuildersMapped;

        private readonly Assembly _modelAssembly;

        /// <summary>
        /// Modify UpdateAt field on saving with current timestamp (defaults to true)
        /// </summary>
        public bool RefreshUpdateAtFieldOnSaving { get; set; } = true;

        protected ModelRepositoryDbContext(DbContextOptions options, Assembly modelAssembly)
            : base(options) => _modelAssembly = modelAssembly;

        /// <summary>
        /// See DbContext.SaveChanges
        /// </summary>
        /// <returns>Rows affected</returns>
        public override int SaveChanges()
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChanges();
        }

        /// <summary>
        /// See DbContext.SaveChanges
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">See DbContext.SaveChanges</param>
        /// <returns>Rows affected</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// See DbContext.SaveChangesAsync
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">See DbContext.SaveChangesAsync</param>
        /// <param name="cancellationToken">See DbContext.SaveChangesAsync</param>
        /// <returns>Rows affected</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// See DbContext.SaveChangesAsync
        /// </summary>
        /// <param name="cancellationToken">See DbContext.SaveChangesAsync</param>
        /// <returns>Rows affected</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        private void DbModelUpdateCreatedAtUpdatedAtFields()
        {
            var entities = ChangeTracker.Entries().Where(entity => (entity.Entity is TimeStampedModel) &&
                ((entity.State == EntityState.Added) || (entity.State == EntityState.Modified)));

            foreach (var entity in entities)
            {
                if (entity.Entity.GetType().IsSubclassOf(typeof(TimeStampedModel)))
                {
                    var now = DateTimeOffset.UtcNow;

                    if ((entity.State == EntityState.Added) || (RefreshUpdateAtFieldOnSaving))
                    {
                        UpdatedAtProperty?.SetValue(entity.Entity, now);
                    }

                    if (entity.State == EntityState.Added)
                    {
                        CreatedAtProperty?.SetValue(entity.Entity, now);
                    }
                }

                if (entity.State == EntityState.Added)
                {
                    var uuidProperty = entity.Entity.GetType().IsSubclassOf(typeof(UuidAwareEnabledAwareTimeStampedModel)) ? UuidEnabledTimeStampedProperty :
                        entity.Entity.GetType().IsSubclassOf(typeof(UuidAwareTimeStampedModel)) ? UuidTimeStampedProperty :
                        entity.Entity.GetType().IsSubclassOf(typeof(UuidAwareEnabledAwareModel)) ? UuidEnabledProperty :
                        entity.Entity.GetType().IsSubclassOf(typeof(UuidAwareModel)) ? UuidProperty :
                        null;

                    var value = uuidProperty?.GetValue(entity.Entity);

                    if (((value is Guid guid) && (guid == Guid.Empty)) || (value is null))
                    {
                        value = Guid.NewGuid();

                        uuidProperty?.SetValue(entity.Entity, value);
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!_modelBuildersMapped)
            {
                ModelBuilderMapper.MapModelBuilders(modelBuilder, _modelAssembly);

                _modelBuildersMapped = true;
            }
        }
    }
}
