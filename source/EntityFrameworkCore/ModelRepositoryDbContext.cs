using Microsoft.EntityFrameworkCore;
using FFCEI.Microservices.Models;
using System.Reflection;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public abstract class ModelRepositoryDbContext : DbContext
    {
        private static readonly PropertyInfo? CreatedAtProperty = typeof(TimeStampedModel).GetProperty("CreatedAt");
        private static readonly PropertyInfo? UpdatedAtProperty = typeof(TimeStampedModel).GetProperty("UpdatedAt");

        private bool _modelBuildersMapped;

        private readonly Assembly _modelAssembly;

        public bool RefreshUpdateAtFieldOnSaving { get; set; } = true;

        protected ModelRepositoryDbContext(DbContextOptions options, Assembly modelAssembly)
            : base(options) => _modelAssembly = modelAssembly;

        public override int SaveChanges()
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DbModelUpdateCreatedAtUpdatedAtFields();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void DbModelUpdateCreatedAtUpdatedAtFields()
        {
            var entities = ChangeTracker.Entries().Where(entity => (entity.Entity is TimeStampedModel) &&
                ((entity.State == EntityState.Added) || (entity.State == EntityState.Modified)));

            foreach (var entity in entities)
            {
                var now = DateTimeOffset.UtcNow;

                if (UpdatedAtProperty is not null)
                {
                    if ((entity.State == EntityState.Added) || (RefreshUpdateAtFieldOnSaving))
                    {
                        UpdatedAtProperty.SetValue(entity.Entity, now);
                    }
                }

                if (entity.State == EntityState.Added)
                {
                    if (CreatedAtProperty is not null)
                    {
                        CreatedAtProperty.SetValue(entity.Entity, now);
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
