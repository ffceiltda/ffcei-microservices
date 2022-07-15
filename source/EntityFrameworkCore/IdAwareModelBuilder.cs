using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Model Builder for IIdAwareModel derived model classes
    /// </summary>
    public static class IdAwareModelBuilder
    {
        /// <summary>
        /// Register IIdAwareModel properties
        /// </summary>
        /// <typeparam name="TEntity">Model entity type</typeparam>
        /// <param name="entityBuilder">Entity builder</param>
        /// <exception cref="ArgumentNullException">Throw null if entityBuilder is null</exception>
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, IIdAwareModel
        {
            if (entityBuilder == null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.Id)
                .IsLongColumn()
                .ValueGeneratedOnAdd();

            entityBuilder.HasKey(e => e.Id)
                .HasName($"{entityBuilder.Metadata.GetTableName()}_PK");
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
