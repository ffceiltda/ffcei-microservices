using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Services;

/// <summary>
/// Apí service base abstract class
/// </summary>
public abstract class ApiService : IApiService
{
    /// <summary>
    /// Protected base constructor
    /// </summary>
    /// <param name="logger">Service logger</param>
    protected ApiService(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Logger instance
    /// </summary>
    public ILogger Logger { get; private set; }
}
