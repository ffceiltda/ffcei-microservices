namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface with Uuid property and IsEnabled property
/// </summary>
public class UuidAwareEnabledAwareModel : EnabledAwareModel, IUuidAwareModel
{
    public Guid Uuid { get; set; }

    public override void CopyModelPropertiesFrom(IModel model)
    {
        base.CopyModelPropertiesFrom(model);

        if (model is IUuidAwareModel modelCasted)
        {
            if (modelCasted.Uuid != Guid.Empty)
            {
                Uuid = modelCasted.Uuid;
            }
        }
    }

    public int CompareTo(IUuidAwareModel? other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return Uuid.CompareTo(other.Uuid);
    }

    public virtual int CompareTo(object? obj)
    {
        if (obj is IUuidAwareModel modelCasted)
        {
            return CompareTo(modelCasted);
        }

        throw new ArgumentException("incompatible object for comparison", nameof(obj));
    }

    public bool Equals(IUuidAwareModel? other) => (other is not null) && (Uuid == other.Uuid);

    public override bool Equals(object? obj)
    {
        if (obj is IUuidAwareModel modelCasted)
        {
            return Equals(modelCasted);
        }

        return false;
    }

    public override int GetHashCode() => Uuid.GetHashCode();

    /// <summary>
    /// Operator ==
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if equals, false otherwise</returns>
    public static bool operator ==(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Operator !=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if different, false otherwise</returns>
    public static bool operator !=(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Operator &lt;
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is less than right, false otherwise</returns>
    public static bool operator <(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Operator &lt;=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is less or equals than right, false otherwise</returns>
    public static bool operator <=(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Operator &gt;
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is greater than right, false otherwise</returns>
    public static bool operator >(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Operator &gt;=
    /// </summary>
    /// <param name="left">left instance</param>
    /// <param name="right">right instance</param>
    /// <returns>true if left is greater or equals than right, false otherwise</returns>
    public static bool operator >=(UuidAwareEnabledAwareModel left, UuidAwareEnabledAwareModel right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
