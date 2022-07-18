using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Services
{
    /// <summary>
    /// Application service base abstract class
    /// </summary>
    public abstract class ApplicationService : IApplicationService
    {
        /// <summary>
        /// Protected base constructor
        /// </summary>
        /// <param name="logger">Service logger</param>
        protected ApplicationService(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; private set; }
    }
}
