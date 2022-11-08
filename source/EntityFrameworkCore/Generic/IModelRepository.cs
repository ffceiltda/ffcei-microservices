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
    /// Update existing Models in repository
    /// </summary>
    /// <param name="models">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task UpdateManyAsync(IEnumerable<TModel> models, bool autoCommit = true);

    /// <summary>
    /// Logically delete existing ModelS by predicate from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="predicate">Predicate for match</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>A queryable instance</returns>
    Task<IEnumerable<TModel>> LogicallyDeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true);

    /// <summary>
    /// Logically delete a existing Model from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyDeleteExistingAsync(TModel model, bool autoCommit = true);

    /// <summary>
    /// Logically delete existing ModelS from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="models">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyDeleteManyAsync(IEnumerable<TModel> models, bool autoCommit = true);

    /// <summary>
    /// Logically undelete existing ModelS by predicate from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="predicate">Predicate for match</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>A queryable instance</returns>
    Task<IEnumerable<TModel>> LogicallyUndeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true);

    /// <summary>
    /// Logically undelete a existing Model from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyUndeleteExistingAsync(TModel model, bool autoCommit = true);

    /// <summary>
    /// Logically undelete existing ModelS from repository if TModel implements ILogicallyDeletableModel
    /// </summary>
    /// <param name="models">Collection of Model instances</param>
    /// <param name="autoCommit">Save changes after operation succeeds</param>
    /// <returns>void</returns>
    Task LogicallyUndeleteManyAsync(IEnumerable<TModel> models, bool autoCommit = true);

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
