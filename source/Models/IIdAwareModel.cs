namespace FFCEI.Microservices.Models
{
    public interface IIdAwareModel : IModel, IEquatable<IIdAwareModel>, IComparable<IIdAwareModel>, IComparable
    {
        long Id { get; }
    }
}
