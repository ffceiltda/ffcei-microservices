using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Model Builder for ILogicallyDeletableModel derived model classes
/// </summary>
public static class LogicallyDeletableModelBuilder
{
    /// <summary>
    /// Register ILogicallyDeletableModel properties
    /// </summary>
    /// <typeparam name="TEntity">Model entity type</typeparam>
    /// <param name="entityBuilder">Entity builder</param>
    /// <param name="databaseEngine">Database engine</param>
    /// <param name="filterLogicallyDeletedRows">Filter logically deleted rows on queries (defaults to true)</param>
    /// <exception cref="ArgumentNullException">Throw null if entityBuilder is null</exception>
    public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DatabaseEngine databaseEngine, bool filterLogicallyDeletedRows = true) where TEntity : class, ILogicallyDeletableModel
    {
        if (entityBuilder is null)
        {
            throw new ArgumentNullException(nameof(entityBuilder));
        }

#pragma warning disable IDE0058 // Expression value is never used
        entityBuilder.Property(e => e.IsLogicallyDeleted)
            .IsRequired()
            .IsBooleanColumn();

        if (filterLogicallyDeletedRows)
        {
            entityBuilder
               .HasQueryFilter(p => !p.IsLogicallyDeleted);
        }
#pragma warning restore IDE0058 // Expression value is never used
    }
}
