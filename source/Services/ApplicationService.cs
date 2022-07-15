using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Services
{
    /// <summary>
    /// Application service base abstract class
    /// </summary>
    public abstract class ApplicationService : IApplicationService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Protected base constructor
        /// </summary>
        /// <param name="logger">Service logger</param>
        protected ApplicationService(ILogger logger)
        {
            _logger = logger;
        }

        public ILogger Logger => _logger;
    }
}
