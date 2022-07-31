using FFCEI.Microservices.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Microservices;

/// <summary>
/// Microservice interface class
/// </summary>
public interface IMicroservice
{
    /// <summary>
    /// Command line arguments
    /// </summary>
    string[] CommandLineArguments { get; }

    /// <summary>
    /// Microservice host builder
    /// </summary>
    IHostBuilder Builder { get; }

    /// <summary>
    /// Microservice Dependency Injector service collection
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Microservice host application (built when called, with Builder settings)
    /// </summary>
    IHost Application { get; }

    /// <summary>
    /// Microservice configuration manager
    /// </summary>
    IConfigurationManager ConfigurationManager { get; }

    /// <summary>
    /// Microservice default logger
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Run service
    /// </summary>
    void Run();

    /// <summary>
    /// Run service, asynchornously
    /// </summary>
    /// <returns></returns>
    Task RunAsync();
}
