namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Native database field types
/// </summary>
public enum DatabaseColumnType
{
    /// <summary>
    /// Binary UUID
    /// </summary>
    Uuid,
    /// <summary>
    /// Boolean
    /// </summary>
    Boolean,
    /// <summary>
    /// Date-only
    /// </summary>
    Date,
    /// <summary>
    /// Time-only
    /// </summary>
    Time,
    /// <summary>
    /// Time-only
    /// </summary>
    TimeSpan,
    /// <summary>
    /// Date and time
    /// </summary>
    DateTime,
    /// <summary>
    /// Date and time (with UTC offset)
    /// </summary>
    DateTimeWithUtcOffset,
    /// <summary>
    /// 64-bit integer
    /// </summary>
    BigInt,
    /// <summary>
    /// Numeric/Decimal number
    /// </summary>
    Numeric,
    /// <summary>
    /// Long Text / Memo
    /// </summary>
    Text,
    /// <summary>
    /// Binary / Blob
    /// </summary>
    Binary
}
