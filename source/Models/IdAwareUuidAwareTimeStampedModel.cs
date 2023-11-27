namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface with Id property and Uuid property and Timestamping (created / updated) support
/// </summary>
public class IdAwareUuidAwareTimestampedModel : UuidAwareTimestampedModel, IIdAwareModel
{
    public long Id { get; set; }

    public override void CopyModelPropertiesFrom(IModel model)
    {
        base.CopyModelPropertiesFrom(model);

        if (model is IIdAwareModel modelCastedId)
        {
            if (modelCastedId.Id != 0)
            {
                Id = modelCastedId.Id;
            }
        }
    }

    public int CompareTo(IIdAwareModel? other)
    {
        ArgumentNullException.ThrowIfNull(other, nameof(other));

        return Id.CompareTo(other.Id);
    }

    public override int CompareTo(object? obj)
    {
        if (obj is IIdAwareModel modelCasted)
        {
            return CompareTo(modelCasted);
        }

        throw new ArgumentException("incompatible object for comparison", nameof(obj));
    }

    public bool Equals(IIdAwareModel? other) => (other is not null) && (Id == other.Id);

    public override bool Equals(object? obj)
    {
        if (obj is IUuidAwareModel modelCasted)
        {
            return Equals(modelCasted);
        }

        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Operator ==
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if equals, false otherwise</returns>
    public static bool operator ==(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
    }

    /// <summary>
    /// Operator !=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if different, false otherwise</returns>
    public static bool operator !=(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Operator &lt;
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is less than right, false otherwise</returns>
    public static bool operator <(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Operator &lt;=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is less or equals than right, false otherwise</returns>
    public static bool operator <=(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Operator &gt;
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is greater than right, false otherwise</returns>
    public static bool operator >(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Operator &gt;=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is greater or equals than right, false otherwise</returns>
    public static bool operator >=(IdAwareUuidAwareTimestampedModel left, IIdAwareModel right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
