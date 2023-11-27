using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Model Builder for ITimestampedModel derived model classes
/// </summary>
public static class TimestampedRodelBuilder
{
    /// <summary>
    /// Register ITimestampedModel properties
    /// </summary>
    /// <typeparam name="TEntity">Model entity type</typeparam>
    /// <param name="entityBuilder">Entity builder</param>
    /// <param name="databaseEngine">Database engine</param>
    /// <exception cref="ArgumentNullException">Throw null if entityBuilder is null</exception>
    public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DatabaseEngine databaseEngine) where TEntity : class, ITimestampedModel
    {
        ArgumentNullException.ThrowIfNull(entityBuilder, nameof(entityBuilder));

#pragma warning disable IDE0058 // Expression value is never used
        entityBuilder.Property(e => e.CreatedAt)
            .IsRequired()
            .IsDateTimeWithUtcOffsetColumn()
            .ValueGeneratedOnAdd();

        entityBuilder.Property(e => e.UpdatedAt)
            .IsRequired()
            .IsDateTimeWithUtcOffsetColumn()
            .ValueGeneratedOnAddOrUpdate();
#pragma warning restore IDE0058 // Expression value is never used
    }
}
