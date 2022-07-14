namespace FFCEI.Microservices.Models
{
    public abstract class UuidAwareModel : Model, IUuidAwareModel
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

        public static bool operator ==(UuidAwareModel left, UuidAwareModel right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(UuidAwareModel left, UuidAwareModel right)
        {
            return !(left == right);
        }

        public static bool operator <(UuidAwareModel left, UuidAwareModel right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(UuidAwareModel left, UuidAwareModel right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(UuidAwareModel left, UuidAwareModel right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(UuidAwareModel left, UuidAwareModel right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
