using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore.Generic;

/// <summary>
/// Generic Model Repository interface
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IModelRepository<TModel> : IReadOnlyModelRepository<TModel>, IModelRepository where TModel : IModel
{
    /// <summary>
    /// Add new Model to repository
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task AddNewAsync(TModel model, bool autoCommit = true);

    /// <summary>
    /// Add new Models to repository
    /// </summary>
    /// <param name="models">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task AddManyAsync(IEnumerable<TModel> models, bool autoCommit = true);

    /// <summary>
    /// Update existing Model in repository
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task UpdateExistingAsync(TModel model, bool autoCommit = true);

    /// <summary>
    /// Remove existing Model from repository
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task RemoveExistingAsync(TModel model, bool autoCommit = true);

    /// <summary>
    /// Remove existing Models from repository
    /// </summary>
    /// <param name="models">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns></returns>
    Task RemoveManyAsync(IEnumerable<TModel> models, bool autoCommit = true);

    /// <summary>
    /// Remove existing Model from repository that match predicate 
    /// </summary>
    /// <param name="predicate">Predicate to match</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task RemoveManyByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true);

    /// <summary>
    /// Reload existing Model from repository
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <returns>void</returns>
    Task ReloadExistingAsync(TModel model);

    /// <summary>
    /// Detach existing Model from repository
    /// </summary>
    /// <param name="model">Model instance</param>
    void Detach(TModel model);
}
