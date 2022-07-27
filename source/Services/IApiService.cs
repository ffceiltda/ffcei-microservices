using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Services;

/// <summary>
/// Apí service interface
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Apí service logger
    /// </summary>
    ILogger Logger { get; }
}
