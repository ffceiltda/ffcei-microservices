using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// PropertyBuilder extension methods
    /// </summary>
    public static class PropertyBuilderExtensionMethods
    {
        /// <summary>
        /// Define a Uuid column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsUuidColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.BinaryUuid)).HasMaxLength(16).IsFixedLength(true);

        /// <summary>
        /// Define a Boolean column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsBooleanColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Boolean));

        /// <summary>
        /// Define a Date column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsDateColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Date));

        /// <summary>
        /// Define a DateTime column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsDateTimeColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.DateTime));

        /// <summary>
        /// Define a TimeSpan column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsTimeSpanColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.TimeSpan));

        /// <summary>
        /// Define a Timestamp column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsTimestampColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Timestamp));

        /// <summary>
        /// Define a High Resolution Timestamp column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsHighResolutionTimestampColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.HighResolutionTimestamp));

        /// <summary>
        /// Define a long integer column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsLongColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.BigInt));

        /// <summary>
        /// Define a numeric (decimal) column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <param name="precision">Numeric precision</param>
        /// <param name="scale">Numeric scale</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsDecimalColumn<TProperty>(this PropertyBuilder<TProperty> property, int? precision = null, int? scale = null) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Numeric, precision, scale)).HasPrecision(precision ?? 20, scale ?? 2);

        /// <summary>
        /// Define a long text (memo / blob) column
        /// </summary>
        /// <typeparam name="TProperty">Model Property</typeparam>
        /// <param name="property">PropertyBuilder instance</param>
        /// <returns>PropertyBuilder instance</returns>
        public static PropertyBuilder<TProperty> IsLongTextColumn<TProperty>(this PropertyBuilder<TProperty> property) => property.HasColumnType(DbNativeFieldTypeMapping.GetNativeMapping(null, DbFieldType.Text));
    }
}
