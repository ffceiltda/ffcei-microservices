namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class DbNativeFieldTypeMapping
    {
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
