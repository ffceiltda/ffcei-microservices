namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface with Id property
/// </summary>
public interface IIdAwareModel : IModel, IEquatable<IIdAwareModel>, IComparable<IIdAwareModel>, IComparable
{
    /// <summary>
    /// Id property
    /// </summary>
    long Id { get; }
}
