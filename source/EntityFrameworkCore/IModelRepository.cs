using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Model repository interface
/// </summary>
public interface IModelRepository : IReadOnlyModelRepository
{
    /// <summary>
    /// Add new Model to repository
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task AddNewAsync(IModel content, bool autoCommit = true);

    /// <summary>
    /// Add new Models to repository
    /// </summary>
    /// <param name="contents">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task AddManyAsync(ICollection<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Update existing Model in repository
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task UpdateExistingAsync(IModel content, bool autoCommit = true);

    /// <summary>
    /// Update existing Models in repository
    /// </summary>
    /// <param name="contents">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task UpdateManyAsync(ICollection<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Logically delete first model that match keys or null
    /// </summary>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <param name="keys">Model lookup keys</param>
    /// <returns>First model that match keys or null</returns>
    ValueTask<IModel?> LogicallyDeleteByKeyAsync(bool autoCommit, params object[] keys);

    /// <summary>
    /// Logically delete a existing Model from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyDeleteExistingAsync(IModel content, bool autoCommit = true);

    /// <summary>
    /// Logically delete existing ModelS from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="contents">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyDeleteManyAsync(ICollection<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Logically undelete first model that match keys or null
    /// </summary>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <param name="keys">Model lookup keys</param>
    /// <returns>First model that match keys or null</returns>
    ValueTask<IModel?> LogicallyUndeleteByKeyAsync(bool autoCommit, params object[] keys);

    /// <summary>
    /// Logically undelete a existing Model from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyUndeleteExistingAsync(IModel content, bool autoCommit = true);

    /// <summary>
    /// Logically undelete existing ModelS from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="contents">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyUndeleteManyAsync(ICollection<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Remove existing Model from repository
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task RemoveExistingAsync(IModel content, bool autoCommit = true);

    /// <summary>
    /// Remove existing Model from repository by keys
    /// </summary>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <param name="keys">Model lookup keys</param>
    /// <returns>void</returns>
    Task RemoveByKeyAsync(bool autoCommit, params object[] keys);

    /// <summary>
    /// Remove existing Models from repository
    /// </summary>
    /// <param name="contents">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns></returns>
    Task RemoveManyAsync(ICollection<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Reload existing Model from repository
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <returns>void</returns>
    Task ReloadExistingAsync(IModel content);

    /// <summary>
    /// Detach existing Model from repository
    /// </summary>
    /// <param name="content">Model instance</param>
    void Detach(IModel content);
}
