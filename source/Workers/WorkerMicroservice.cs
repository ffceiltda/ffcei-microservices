using FFCEI.Microservices.Microservices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCEI.Microservices.Workers;

/// <summary>
/// Worker microservice template
/// </summary>
public class WorkerMicroservice : MicroserviceBase
{
#pragma warning disable CA1000
    /// <summary>
    /// Microservice instance (singleton)
    /// </summary>
    public static new WorkerMicroservice? Instance => MicroserviceBase.Instance as WorkerMicroservice;
#pragma warning restore CA1000

    /// <summary>
    /// Worker Microservice constructor
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments</param>
    public WorkerMicroservice(string[] commandLineArguments)
        : base(commandLineArguments)
    {
        _applicationBuilder = WebApplication.CreateBuilder(commandLineArguments);
        _initialBuilder = _applicationBuilder.Host;

        MicroserviceName = _applicationBuilder.Environment.ApplicationName;
    }
    
    public override void Run()
    {
        Application.Run("http://127.0.0.1:0");
    }

    public override async Task RunAsync()
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await Application.RunAsync("http://127.0.0.1:0");
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
