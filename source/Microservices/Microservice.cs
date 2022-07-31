using FFCEI.Microservices.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.Microservices;

public abstract class Microservice : IMicroservice
{
    private readonly string[] _commandLineArguments;
    private IHostBuilder? _builder;
    private IServiceCollection? _services;
    private IHost? _application;
    private IConfigurationManager? _configurationManager;
    private ILogger? _logger;

    protected Microservice(string[] commandLineArguments)
    {
        _commandLineArguments = commandLineArguments;
    }

    string[] IMicroservice.CommandLineArguments => _commandLineArguments;

    IHostBuilder IMicroservice.Builder => _builder ??= GetImplementationBuilder();

    IServiceCollection IMicroservice.Services => _services ??= GetImplementationServices();

    IHost IMicroservice.Application => _application ??= GetImplementationApplication();

    IConfigurationManager IMicroservice.ConfigurationManager => _configurationManager ??= GetConfigurationManager();

    ILogger IMicroservice.Logger => _logger ??= GetLogger();

    protected abstract IHostBuilder GetImplementationBuilder();
    protected abstract IServiceCollection GetImplementationServices();
    protected abstract IHost GetImplementationApplication();

    private IConfigurationManager GetConfigurationManager()
    {
        throw new NotImplementedException();
    }

    private ILogger GetLogger()
    {
        throw new NotImplementedException();
    }

    public abstract void Run();

    public abstract Task RunAsync();
}
