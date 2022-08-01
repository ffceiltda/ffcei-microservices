using FFCEI.Microservices.Microservices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCEI.Microservices.Workers;

/// <summary>
/// Woker Microservice
/// </summary>
public class WorkerMicroservice : Microservice
{
    private IHostBuilder? _initialBuilder;
    private IServiceCollection? _services;

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

    protected override IServiceCollection GetImplementationServices()
    {
        var builder = _initialBuilder ?? Builder;

#pragma warning disable IDE0058 // Expression value is never used
        builder.ConfigureServices((context, services) =>
        {
            _services = services;
        });
#pragma warning restore IDE0058 // Expression value is never used

        if (_services is null)
        {
            throw new InvalidOperationException("ServiceCollection cannot be instantiated");
        }

        return _services;
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
