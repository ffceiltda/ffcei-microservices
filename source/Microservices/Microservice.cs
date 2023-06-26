using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCEI.Microservices.Microservices;

/// <summary>
/// Microservice impplementation
/// </summary>
public class Microservice : MicroserviceBase
{
    private IHostBuilder? _initialBuilder;
    private WebApplicationBuilder? _applicationBuilder;
    private WebApplication? _application;

#pragma warning disable CA1000
    /// <summary>
    /// Microservice instance (singleton)
    /// </summary>
    public static new Microservice? Instance => MicroserviceBase.Instance as Microservice;
#pragma warning restore CA1000

    /// <summary>
    /// Microservice constructor
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments</param>
    public Microservice(string[] commandLineArguments)
        : base(commandLineArguments)
    {
        _applicationBuilder = WebApplication.CreateBuilder(commandLineArguments);
        _initialBuilder = _applicationBuilder.Host;

        MicroserviceName = _applicationBuilder.Environment.ApplicationName;
    }

    protected override IHostBuilder? GetImplementationInitialBuilder()
    {
        return _initialBuilder;
    }

    protected override IServiceCollection GetImplementationServices()
    {
        var result = _applicationBuilder?.Services;

        if (result is null)
        {
            throw new InvalidOperationException("Microservice GetImplementationServices() detected a internal error");
        }

        return result;
    }

    protected override IHostEnvironment GetImplementationEnvironment()
    {
        var result = _application?.Environment ?? _applicationBuilder?.Environment;

        if (result is null)
        {
            throw new InvalidOperationException("Microservice GetImplementationEnvironment() detected a internal error");
        }

        return result;
    }

    private WebApplication CreateWebApplication()
    {
        if (_application is not null)
        {
            throw new InvalidOperationException("Web Api Microservice CreateWebApplication() was already called before");
        }

        var builder = _applicationBuilder;

        if ((builder is null) || (Builder is null))
        {
            throw new InvalidOperationException("Web Api Microservice CreateWebApplication() logic error");
        }

        _application = builder.Build();

        return _application;
    }

    protected override IHost GetImplementationHost() => Application;

    protected WebApplication Application => _application ??= CreateWebApplication();
}
