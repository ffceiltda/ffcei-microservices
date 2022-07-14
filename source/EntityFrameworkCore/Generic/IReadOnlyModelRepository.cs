using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore.Generic
{
    public interface IReadOnlyModelRepository<TModel> : IReadOnlyModelRepository where TModel : IModel
    {
        Task<IReadOnlyList<TModel>> AllAsync();

        ValueTask<TModel?> FirstOrDefaultByPredicateAsync(Expression<Func<TModel, bool>> predicate);

        Task<IReadOnlyList<TModel>> ManyByPredicateAsync(Expression<Func<TModel, bool>> predicate);
    }
}
