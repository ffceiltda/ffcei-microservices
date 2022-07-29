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

    /// <summary>
    /// Logically delete a model instance
    /// </summary>
    void LogicallyDelete();

    /// <summary>
    /// Logically undelete a model instance
    /// </summary>
    void LogicallyUndelete();
}
