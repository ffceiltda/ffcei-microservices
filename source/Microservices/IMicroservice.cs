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
    /// Microservice name
    /// </summary>
    string MicroserviceName { get; }

    /// <summary>
    /// Command line arguments
    /// </summary>
    IReadOnlyList<string> CommandLineArguments { get; }

    /// <summary>
    /// Microservice host builder
    /// </summary>
    IHostBuilder Builder { get; }

    /// <summary>
    /// Microservice host Environment
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// Microservice Dependency Injector service collection
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Microservice host application (built when called, with Builder settings)
    /// </summary>
    IHost Host { get; }

    /// <summary>
    /// Microservice configuration manager
    /// </summary>
    IConfigurationManager ConfigurationManager { get; }

    /// <summary>
    /// Microservice default logger
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Return true if is debugging or if it´s running in develoment environment
    /// </summary>
    bool IsDebugOrDevelopment { get; }

    /// <summary>
    /// Machine configuration search path
    /// </summary>
    public string? ConfigurationMachineSearchPath { get; set; }

    /// <summary>
    /// User configuration search path
    /// </summary>
    public string? ConfigurationUserSearchPath { get; set; }

    /// <summary>
    /// Registry path for configuration (search first in HKCU, then HKLM)
    /// </summary>
    public string? RegistryPathForConfiguration { get; set; }

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
