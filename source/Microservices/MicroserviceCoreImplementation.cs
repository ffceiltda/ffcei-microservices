using FFCEI.Microservices.Microservices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCEI.Microservices.Microservices;

/// <summary>
/// Worker microservice template
/// </summary>
public class MicroserviceCoreImplementation : Microservice
{
    private IHostBuilder? _initialBuilder;
    private WebApplicationBuilder? _applicationBuilder;
    private WebApplication? _application;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments</param>
    public MicroserviceCoreImplementation(string[] commandLineArguments)
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
            throw new InvalidOperationException("Microservice GetImplementationEnvironment() detected a internal error");
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

    protected WebApplication Application => _application ??= CreateWebApplication();

    protected override IHost GetImplementationHost() => Application;
}
