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
    Task AddManyAsync(IEnumerable<IModel> contents, bool autoCommit = true);

    /// <summary>
    /// Update existing Model in repository
    /// </summary>
    /// <param name="content">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task UpdateExistingAsync(IModel content, bool autoCommit = true);

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
    Task RemoveManyAsync(IEnumerable<IModel> contents, bool autoCommit = true);

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
