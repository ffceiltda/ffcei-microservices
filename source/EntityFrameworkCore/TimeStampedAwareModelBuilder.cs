using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class TimeStampedAwareModelBuilder
    {
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, ITimeStampedModel
        {
            if (entityBuilder == null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.CreatedAt)
                .IsRequired()
                .IsHighResolutionTimestampColumn();

            entityBuilder.Property(e => e.UpdatedAt)
                .IsRequired()
                .IsHighResolutionTimestampColumn();
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
