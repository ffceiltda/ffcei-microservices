using FFCEI.Microservices.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCEI.Microservices.Microservices;

public interface IMicroservice
{
    IHostBuilder Builder { get; }
    IServiceCollection Services { get; }
    IHost Application { get; }
    IConfigurationManager ConfigurationManager { get; }
    void Run();
    Task RunAsync();
}
