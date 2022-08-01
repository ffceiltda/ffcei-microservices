using Destructurama;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Json;
using FFCEI.Microservices.Microservices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.Workers;

/// <summary>
/// Woker Microservice
/// </summary>
public class WorkerMicroservice : Microservice
{
    private IHostBuilder? _initialBuilder;

    /// <summary>
    /// Worker Microservice constructor
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments</param>
    public WorkerMicroservice(string[] commandLineArguments)
        : base(commandLineArguments)
    {
        _initialBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(commandLineArguments);
    }

    protected override IHostBuilder? GetImplementationInitialBuilder()
    {
        return _initialBuilder;
    }

    protected override IHostEnvironment GetImplementationEnvironment()
    {
        var result = Host.Services.GetRequiredService<IHostEnvironment>();

        return result;
    }

    protected override IHost GetImplementationHost()
    {
        if (_initialBuilder is null)
        {
            throw new InvalidOperationException("Microservice GetImplementationApplication() was already called before");
        }

        var result = Builder.Build();

        _initialBuilder = null!;

        return result;
    }
}
