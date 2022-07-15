namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Native database field types
    /// </summary>
    public enum DbFieldType
    {
        /// <summary>
        /// Binary UUID
        /// </summary>
        BinaryUuid,
        /// <summary>
        /// Boolean
        /// </summary>
        Boolean,
        /// <summary>
        /// Date-only
        /// </summary>
        Date,
        /// <summary>
        /// Date and time
        /// </summary>
        DateTime,
        /// <summary>
        /// Time-only
        /// </summary>
        TimeSpan,
        /// <summary>
        /// Precision date and time
        /// </summary>
        TimeStamp,
        /// <summary>
        /// High resolution precision date and time
        /// </summary>
        HighResolutionTimeStamp,
        /// <summary>
        /// 64-bit integer
        /// </summary>
        BigInt,
        /// <summary>
        /// Numeric/Decimal number
        /// </summary>
        Numeric,
        /// <summary>
        /// Blob / LongText
        /// </summary>
        Text
    }
}
