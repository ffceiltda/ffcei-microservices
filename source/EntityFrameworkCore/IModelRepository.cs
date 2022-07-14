using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public interface IModelRepository : IReadOnlyModelRepository
    {
        Task AddNewAsync(IModel content, bool autoCommit = true);

        Task AddNewRangeAsync(IEnumerable<IModel> contents, bool autoCommit = true);

        Task UpdateExistingAsync(IModel content, bool autoCommit = true);

        Task RemoveExistingAsync(IModel content, bool autoCommit = true);

        Task RemoveByKeyAsync(bool autoCommit, params object[] keys);

        Task ReloadExistingAsync(IModel content);

        void Detach(IModel content);
    }
}
