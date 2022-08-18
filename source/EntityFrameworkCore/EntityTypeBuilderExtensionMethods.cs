using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace FFCEI.Microservices.EntityFrameworkCore;

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
