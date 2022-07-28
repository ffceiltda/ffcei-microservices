namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface with IsLogicallyDeleted property
/// </summary>
public interface ILogicallyDeletableModel : IModel
{
    /// <summary>
    /// IsLogicallyDeleted property
    /// </summary>
    bool IsLogicallyDeleted { get; set; }
}
