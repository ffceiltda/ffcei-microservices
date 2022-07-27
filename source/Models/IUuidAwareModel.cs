namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface with Uuid property
/// </summary>
public interface IUuidAwareModel : IModel, IEquatable<IUuidAwareModel>, IComparable<IUuidAwareModel>, IComparable
{
    /// <summary>
    /// Uuid property
    /// </summary>
    Guid Uuid { get; }
}
