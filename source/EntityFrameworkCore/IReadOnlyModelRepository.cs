using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public interface IReadOnlyModelRepository
    {
        Task<IReadOnlyList<IModel>> AllModelsAsync();

        ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys);
    }
}
