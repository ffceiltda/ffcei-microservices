namespace FFCEI.Microservices.Models
{
    public interface IUuidAwareModel : IModel, IEquatable<IUuidAwareModel>, IComparable<IUuidAwareModel>, IComparable
    {
        Guid Uuid { get; }
    }
}
