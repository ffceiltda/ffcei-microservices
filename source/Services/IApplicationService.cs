using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Services
{
    /// <summary>
    /// Application service interface
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Application service logger
        /// </summary>
        ILogger Logger { get; }
    }
}
