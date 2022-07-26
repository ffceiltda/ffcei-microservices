namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Map Db Field Types do SQL language field types
    /// </summary>
    public static class DatabaseColumnTypeMapping
    {
        /// <summary>
        /// Map Db Field Types do SQL language field types
        /// </summary>
        /// <param name="driverType">Database driver</param>
        /// <param name="columnType">Database field type</param>
        /// <param name="precision">Field precision (if applicable)</param>
        /// <param name="scale">Field scale (if applicable)</param>
        /// <returns>SQL language field type</returns>
        /// <exception cref="NotImplementedException">Throws if driver is invalid or unknown</exception>
        public static string GetNativeMapping(Type? driverType, DatabaseColumnType columnType, int? precision = null, int? scale = null)
        {
            if (driverType is not null)
            {
                // TODO: driver type
                throw new NotImplementedException("Unknown driver");
            }

            return columnType switch
            {
                DatabaseColumnType.Uuid => "char(36)",
                DatabaseColumnType.Boolean => "tinyint(1)",
                DatabaseColumnType.Date => "date",
                DatabaseColumnType.Time => "time(6)",
                DatabaseColumnType.TimeSpan => "bigint",
                DatabaseColumnType.DateTime => "datetime(6)",
                DatabaseColumnType.DateTimeWithUtcOffset => "datetime(6)",
                DatabaseColumnType.Numeric => $"decimal({precision ?? 20}, {scale ?? 2})",
                DatabaseColumnType.BigInt => "bigint",
                DatabaseColumnType.Text => "longtext",
                _ => throw new NotImplementedException("Unsupported driverType/fieldType combination for FFCEI.Microservices.EntityFrameworkCore.DbNativeFieldTypeMapping.GetNativeMapping()")
            };
        }
    }
}
