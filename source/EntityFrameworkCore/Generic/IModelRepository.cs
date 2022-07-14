using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore.Generic
{
    public interface IModelRepository<TModel> : IReadOnlyModelRepository<TModel>, IModelRepository where TModel : IModel
    {
        Task AddNewAsync(TModel model, bool autoCommit = true);

        Task AddNewRangeAsync(IEnumerable<TModel> models, bool autoCommit = true);

        Task UpdateExistingAsync(TModel model, bool autoCommit = true);

        Task RemoveExistingAsync(TModel model, bool autoCommit = true);

        Task RemoveManyByPredicateAsync(Expression<Func<TModel, bool>> predicate, bool autoCommit = true);

        Task ReloadExistingAsync(TModel model);

        void Detach(TModel model);
    }
}
