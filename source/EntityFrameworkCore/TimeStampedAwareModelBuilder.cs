using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Model Builder for ITimeStampedModel derived model classes
    /// </summary>
    public static class TimeStampedAwareModelBuilder
    {
        /// <summary>
        /// Register ITimeStampedModel properties
        /// </summary>
        /// <typeparam name="TEntity">Model entity type</typeparam>
        /// <param name="entityBuilder">Entity builder</param>
        /// <param name="databaseEngine">Database engine</param>
        /// <exception cref="ArgumentNullException">Throw null if entityBuilder is null</exception>
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DatabaseEngine databaseEngine) where TEntity : class, ITimeStampedModel
        {
            if (entityBuilder is null)
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
