using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Model Builder for IUuidAwareModel derived model classes
    /// </summary>
    public static class UuidAwareModelBuilder
    {
        /// <summary>
        /// Register IUuidAwareModel properties
        /// </summary>
        /// <typeparam name="TEntity">Model entity type</typeparam>
        /// <param name="entityBuilder">Entity builder</param>
        /// <param name="databaseEngine">Database engine</param>
        /// <exception cref="ArgumentNullException">Throw null if entityBuilder is null</exception>
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DatabaseEngine databaseEngine) where TEntity : class, IUuidAwareModel
        {
            if (entityBuilder is null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.Uuid)
                .IsRequired()
                .IsUuidColumn();

            entityBuilder.HasAlternateKey(e => e.Uuid)
                .HasName($"{entityBuilder.Metadata.GetTableName()}_UK_1");
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
