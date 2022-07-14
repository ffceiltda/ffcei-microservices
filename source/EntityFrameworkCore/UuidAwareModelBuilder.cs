using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class UuidAwareModelBuilder
    {
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, IUuidAwareModel
        {
            if (entityBuilder == null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.Uuid)
                .IsRequired()
                .IsUuidColumn();

            entityBuilder.HasIndex(e => e.Uuid, $"{entityBuilder.Metadata.GetTableName()}_UK_1")
                .IsUnique();
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
