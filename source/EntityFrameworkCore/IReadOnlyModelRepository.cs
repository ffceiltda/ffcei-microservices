using FFCEI.Microservices.Models;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Read-Only Model Repository
    /// </summary>
    public interface IReadOnlyModelRepository
    {
        /// <summary>
        /// Return all models in repository
        /// </summary>
        /// <returns>All models in repository</returns>
        Task<IEnumerable<IModel>> AllModelsAsync();

        /// <summary>
        /// Return first model that match keys or null
        /// </summary>
        /// <param name="keys">Model lookup keys</param>
        /// <returns>First model that match keys or null</returns>
        ValueTask<IModel?> FirstOrDefaultByKeyAsync(params object[] keys);
    }
}
