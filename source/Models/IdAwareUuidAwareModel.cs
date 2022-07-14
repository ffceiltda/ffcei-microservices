namespace FFCEI.Microservices.Models
{
    public abstract class IdAwareUuidAwareModel : UuidAwareModel, IIdAwareModel
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
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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

        public static bool operator ==(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            return !(left == right);
        }

        public static bool operator <(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(IdAwareUuidAwareModel left, IdAwareUuidAwareModel right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
