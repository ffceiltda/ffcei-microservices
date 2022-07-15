namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Map Db Field Types do SQL language field types
    /// </summary>
    public static class DbNativeFieldTypeMapping
    {
        /// <summary>
        /// Map Db Field Types do SQL language field types
        /// </summary>
        /// <param name="driverType">Database driver</param>
        /// <param name="fieldType">Database field type</param>
        /// <param name="precision">Field precision (if applicable)</param>
        /// <param name="scale">Field scale (if applicable)</param>
        /// <returns>SQL language field type</returns>
        /// <exception cref="NotImplementedException">Throws if driver is invalid or unknown</exception>
        public static string GetNativeMapping(Type? driverType, DbFieldType fieldType, int? precision = null, int? scale = null)
        {
            if (driverType is not null)
            {
                // TODO: driver type
                throw new NotImplementedException("Unknown driver");
            }

            return fieldType switch
            {
                DbFieldType.BinaryUuid => "binary(16)",
                DbFieldType.Boolean => "bit(1)",
                DbFieldType.Date => "date",
                DbFieldType.DateTime => "datetime",
                DbFieldType.TimeSpan => "bigint",
                DbFieldType.TimeStamp => "timestamp",
                DbFieldType.HighResolutionTimeStamp => "timestamp(6)",
                DbFieldType.Numeric => $"decimal({precision ?? 20}, {scale ?? 2})",
                DbFieldType.BigInt => "bigint",
                DbFieldType.Text => "longtext",
                _ => throw new NotImplementedException("Unsupported driverType/fieldType combination for FFCEI.Microservices.EntityFrameworkCore.DbNativeFieldTypeMapping.GetNativeMapping()")
            };
        }
    }
}
