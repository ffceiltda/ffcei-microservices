using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Extensions for multiple query filters for entity in ModelBuilder (EntityTypeBuilder&lt;TEntity&gt;)
/// </summary>
public static class EntityTypeBuilderExtensionMethods
{
    private interface IEntityTypeQueryFilterBuilderApplyVisitor
    {
        void Visit();
    }

    private class EntityTypeQueryFilterBuilderApplyVisitor<TEntity> : IEntityTypeQueryFilterBuilderApplyVisitor where TEntity : class, IModel
    {
        public EntityTypeBuilder<TEntity> Builder { get; set; }
        public IEntityTypeQueryFilterBuilder<TEntity> QueryFilterBuilder { get; set; }

        public EntityTypeQueryFilterBuilderApplyVisitor(EntityTypeBuilder<TEntity> builder, IEntityTypeQueryFilterBuilder<TEntity> queryFilterBuilder)
        {
            Builder = builder;
            QueryFilterBuilder = queryFilterBuilder;
        }

        public void Visit()
        {
#pragma warning disable IDE0058 // Expression value is never used
            Builder.HasQueryFilter(QueryFilterBuilder);
#pragma warning restore IDE0058 // Expression value is never used
        }
    }

    private static ConditionalWeakTable<object, object> _extendedData = new ConditionalWeakTable<object, object>();

    /// <summary>
    /// Apply a QueryFilterBuilder into a Entity for Query Filtering
    /// </summary>
    /// <typeparam name="TEntity">IModel derived entity</typeparam>
    /// <param name="entityTypeBuilder">Entity Type Builder</param>
    /// <param name="queryFilterBuilder">Query Filter Builder</param>
    /// <returns>EF.Core Entity Type Builder</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static EntityTypeBuilder<TEntity> HasQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, IEntityTypeQueryFilterBuilder<TEntity> queryFilterBuilder) where TEntity : class, IModel
    {
        if (entityTypeBuilder is null)
        {
            throw new ArgumentNullException(nameof(entityTypeBuilder));
        }

        if (queryFilterBuilder is null)
        {
            throw new ArgumentNullException(nameof(queryFilterBuilder));
        }

        return entityTypeBuilder.HasQueryFilter(queryFilterBuilder.Build());
    }

    /// <summary>
    /// Creates a Query Filter for a ModelBuilder (EntityTypeBuilder&lt;TEntity&gt;)
    /// </summary>
    /// <typeparam name="TEntity">IModel derived entity</typeparam>
    /// <param name="entityTypeBuilder">Entity Type Builder</param>
    /// <returns></returns>
    public static IEntityTypeQueryFilterBuilder<TEntity> QueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, IModel
    {
        if (!_extendedData.TryGetValue(entityTypeBuilder, out var extendedDataObject))
        {
            var queryFilterBuilder = new EntityTypeQueryFilterBuilder<TEntity>();
            var queryFilterApplyVisitor = new EntityTypeQueryFilterBuilderApplyVisitor<TEntity>(entityTypeBuilder, queryFilterBuilder);

            extendedDataObject = new ExpandoObject();

            dynamic dynamicPropertiesCreated = extendedDataObject;

            dynamicPropertiesCreated.QueryFilterBuilder = queryFilterBuilder;
            dynamicPropertiesCreated.QueryFilterApplyVisitor = queryFilterApplyVisitor;

            _extendedData.Add(entityTypeBuilder, extendedDataObject);

            return queryFilterBuilder;
        }

        dynamic dynamicProperties = extendedDataObject;

        return dynamicProperties.QueryFilterBuilder;
    }

    internal static void ApplyQueryFilters()
    {
        foreach (var queryFilters in _extendedData)
        {
            dynamic dynamicProperties = queryFilters.Value;

            dynamicProperties.QueryFilterApplyVisitor.Visit();
        }

        _extendedData.Clear();
    }
}
