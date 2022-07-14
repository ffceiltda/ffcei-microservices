using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class PropertyBuilderExtensionMethods
    {
        public static PropertyBuilder<TProperty> IsUuidColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.BinaryUuid)).HasMaxLength(16).IsFixedLength(true);

        public static PropertyBuilder<TProperty> IsBooleanColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Boolean));

        public static PropertyBuilder<TProperty> IsDateColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Date));

        public static PropertyBuilder<TProperty> IsDateTimeColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.DateTime));

        public static PropertyBuilder<TProperty> IsTimeSpanColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.TimeSpan));

        public static PropertyBuilder<TProperty> IsTimestampColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.TimeStamp));

        public static PropertyBuilder<TProperty> IsHighResolutionTimestampColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.HighResolutionTimeStamp));

        public static PropertyBuilder<TProperty> IsLongColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.BigInt));

        public static PropertyBuilder<TProperty> IsDecimalColumn<TProperty>(this PropertyBuilder<TProperty> property, int? precision = null, int? scale = null) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Numeric, precision, scale)).HasPrecision(precision ?? 20, scale ?? 2);

        public static PropertyBuilder<TProperty> IsLongTextColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Text));
    }
}
