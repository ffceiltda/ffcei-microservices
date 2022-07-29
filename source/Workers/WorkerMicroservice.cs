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
/// microservice template
/// </summary>
public class WorkerMicroservice : IMicroservice
{
    private readonly string[] _args;
    private IHostBuilder? _initialBuilder;
    private IHostBuilder? _builder;
    private IServiceCollection _services = null!;
    private IHost? _application;
    private ConfigurationManager _configurationManager = null!;
    private RedisConnectionConfiguration? _entityFrameworkSecondLevelCacheRedisConfiguration;
    private static WeakReference<WorkerMicroservice> _instance = null!;

    IHostBuilder IMicroservice.Builder => Builder;
    IServiceCollection IMicroservice.Services => Services;
    IHost IMicroservice.Application => Application;
    IConfigurationManager IMicroservice.ConfigurationManager => ConfigurationManager;

    /// <summary>
    /// Host Application Builder
    /// </summary>
    public IHostBuilder Builder => _builder ??= CreateBuilder();

    /// <summary>
    /// Host Dependency Injection Services
    /// </summary>
    public IServiceCollection Services => _services;

    /// <summary>
    /// Host Application
    /// </summary>
    public IHost Application => _application ??= CreateApplication();

    /// <summary>
    /// Configuration Manager (with support for system environment, environment files and ASP.NET Core appSettings)
    /// </summary>
    public ConfigurationManager ConfigurationManager => _configurationManager;

    /// <summary>
    /// Set shutting down to true make all new requests to reply with HTTP Code 503 - Service unavailable, for load balancers
    /// </summary>
    public bool ShuttingDown { get; set; }

    /// <summary>
    /// Json: ignore null values on serialization (default to true)
    /// </summary>
    public bool JsonIgnoreNullOnSerialization { get; set; } = true;

    /// <summary>
    /// Json: write indented on serialization (default to false)
    /// </summary>
    public bool JsonWriteIndented { get; set; }

    /// <summary>
    /// Entity Framework Core: which Second Level Cache method must be used
    /// </summary>
    public EntityFrameworkSecondLevelCacheType EntityFrameworkSecondLevelCacheType { get; set; } = EntityFrameworkSecondLevelCacheType.NoCache;

    /// <summary>
    /// Entity Framework Core: second level cache expiration mode
    /// </summary>
    public CacheExpirationMode EntityFrameworkSecondLevelCacheExpirationMode { get; set; } = CacheExpirationMode.Absolute;

    /// <summary>
    /// Entity Framework Core: second level cache expiration period
    /// </summary>
    public TimeSpan EntityFrameworkSecondLevelCacheExpirationPeriod { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Microservice instance
    /// </summary>

    public static WorkerMicroservice? Instance
    {
        get
        {
            if ((_instance is not null) && _instance.TryGetTarget(out var result))
            {
                return result;
            }

            return null;
        }
    }

    /// <summary>
    /// Microservice constructor
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <exception cref="InvalidOperationException">Throws a invalid operation exception if you try to instantiante more than one instance</exception>
    public WorkerMicroservice(string[] args)
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("Microservice must be instantiated only once");
        }

        _args = args;
        _instance = new WeakReference<WorkerMicroservice>(this);
        _initialBuilder = Host.CreateDefaultBuilder(_args);
        _initialBuilder.ConfigureServices((context, services) =>
            {
                _services = services;
                _configurationManager = new ConfigurationManager(_initialBuilder, context.Configuration);
            });
    }

    private IHostBuilder CreateBuilder()
    {
        if ((_builder is not null) || (_initialBuilder is null))
        {
            throw new InvalidOperationException("Microservice CreateBuilder() was already called before");
        }

        _builder = _initialBuilder;
        _initialBuilder = null;

        OnCreateBuilder();

        return _builder;
    }

    private IHost CreateApplication()
    {
        if (_application is not null)
        {
            throw new InvalidOperationException("Microservice CreateApplication() was already called before");
        }

        _application = Builder.Build();

        return _application;
    }

    /// <summary>
    /// Create Web Application builder settings
    /// </summary>
    /// <exception cref="ArgumentNullException">Throws when builder is null</exception>
    protected virtual void OnCreateBuilder()
    {
        BuildSerilog();
        BuildEntityFramework();
        BuildAutoMapper();
    }

    private ConfigurationManager CreateConfigurationManager(HostBuilderContext context, IServiceCollection services)
    {

        return _configurationManager;
    }

    /// <summary>
    /// Use In-Memory Entity Framework Second Level Cache Provider
    /// </summary>
    /// <exception cref="InvalidOperationException">if HostBuilder is already created</exception>
    public void UseMemoryEntityFrameworkSecondLevelCache()
    {
        if (_builder is not null)
        {
            throw new InvalidOperationException("UseRedisEntityFrameworkSecondLevelCache must be used before access Microservice.Builder property");
        }

        EntityFrameworkSecondLevelCacheType = EntityFrameworkSecondLevelCacheType.MemoryCache;
    }

    /// <summary>
    /// Use In-Memory Entity Framework Second Level Cache Provider
    /// </summary>
    /// <param name="configuration">Redis configuration (optional)</param>
    /// <exception cref="InvalidOperationException">throws if HostBuilder is already created</exception>
    /// <exception cref="ArgumentNullException">throws if redis configuration host or port is null</exception>
    public void UseRedisEntityFrameworkSecondLevelCache(RedisConnectionConfiguration? configuration = null)
    {
        if (_builder is not null)
        {
            throw new InvalidOperationException("UseRedisEntityFrameworkSecondLevelCache must be used before access Microservice.Builder property");
        }

        if (configuration is null)
        {
            var standardConfiguration = ConfigurationManager.GetRedisConfiguration(
                hostSettingName: "Redis.Cache.Host",
                portSettingName: "Redis.Cache.Port",
                usernameSettingName: "Redis.Cache.UserName",
                passwordSettingName: "Redis.Cache.Password",
                databaseSettingName: "Redis.Cache.Database");

            if ((standardConfiguration.Host is null) || (standardConfiguration.Port is null))
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration = standardConfiguration;
        }

        EntityFrameworkSecondLevelCacheType = EntityFrameworkSecondLevelCacheType.RedisCache;

        _entityFrameworkSecondLevelCacheRedisConfiguration = configuration;
    }

    /// <summary>
    /// Run microservice
    /// </summary>
    public void Run()
    {
        Application.Run();
    }

    /// <summary>
    /// Run microservice asynchronously
    /// </summary>
    public async Task RunAsync()
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await Application.RunAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }

#pragma warning disable IDE0058 // Expression value is never used
    private void BuildSerilog()
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        Log.Logger = BuildSeriLogConfiguration(Builder, new LoggerConfiguration()).CreateBootstrapLogger();

        Builder.UseSerilog((context, serviceProvider, configuration) => BuildSeriLogConfiguration(Builder, configuration));
    }

    private static LoggerConfiguration BuildSeriLogConfiguration(IHostBuilder builder, LoggerConfiguration configuration)
    {
        configuration
            .Enrich.WithProperty("Application", Process.GetCurrentProcess().ProcessName)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationIdHeader()
            .Destructure.UsingAttributes()
            .WriteTo.Console();

        if (!Debugger.IsAttached)
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("EFCoreSecondLevelCacheInterceptor", LogEventLevel.Warning);
        }
        else
        {
            configuration
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .MinimumLevel.Override("EFCoreSecondLevelCacheInterceptor", LogEventLevel.Information);
        }

        return configuration;
    }

    private void BuildEntityFramework()
    {
        BuildEntityFrameworkCoreSecondLevelCache();
    }

    private void BuildEntityFrameworkCoreSecondLevelCache()
    {
        switch (EntityFrameworkSecondLevelCacheType)
        {
        case EntityFrameworkSecondLevelCacheType.NoCache:
            {
                break;
            }
        case EntityFrameworkSecondLevelCacheType.MemoryCache:
            {
                BuildMemoryEntityFrameworkCoreSecondLevelCache();

                break;
            }
        case EntityFrameworkSecondLevelCacheType.RedisCache:
            {
                BuildRedisEntityFrameworkCoreSecondLevelCache();

                break;
            }
        default:
            {
                throw new InvalidOperationException(nameof(EntityFrameworkSecondLevelCacheType));
            }
        }
    }

    private void BuildEntityFrameworkCoreSecondLevelCacheOptions(EFCoreSecondLevelCacheOptions options)
    {
        var assemblyPrefix = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0].Replace(".", "", StringComparison.InvariantCulture) ?? string.Empty;

        options
            .UseCacheKeyPrefix($"EFCoreSecondLevelCache_{assemblyPrefix}_")
            .CacheAllQueries(EntityFrameworkSecondLevelCacheExpirationMode, EntityFrameworkSecondLevelCacheExpirationPeriod)
            .SkipCachingResults(result => (result.Value is null) || ((result.Value is EFTableRows rows) && (rows.RowsCount == 0)))
            .DisableLogging(!Debugger.IsAttached);
    }

    private void BuildMemoryEntityFrameworkCoreSecondLevelCache()
    {
        Services.AddEFSecondLevelCache(options =>
        {
            BuildEntityFrameworkCoreSecondLevelCacheOptions(options.UseMemoryCacheProvider());
        });
    }

    private void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
        options.WriteIndented = JsonWriteIndented;
        options.DefaultIgnoreCondition = JsonIgnoreNullOnSerialization ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never;

        options.Converters.Add(new JsonTrimmingConverter());
        options.Converters.Add(new JsonLooseStringEnumConverter());
        options.Converters.Add(new JsonStringToDecimalConverter());
        options.Converters.Add(new JsonStringToLongConverter());
        options.Converters.Add(new JsonStringToIntegerConverter());
    }

    private void BuildRedisEntityFrameworkCoreSecondLevelCache()
    {
        Services.AddEFSecondLevelCache(options =>
        {
            BuildEntityFrameworkCoreSecondLevelCacheOptions(options.UseEasyCachingCoreProvider("EFSecondLevelRedisCache"));
        });

        Services.AddEasyCaching(option =>
        {
            option.WithSystemTextJson((JsonSerializerOptions options) =>
            {
                ConfigureJsonSerializerOptions(options);
            }, "EFSecondLevelJsonSerializer");

            option.UseRedisLock();
            option.UseRedis(config =>
            {
                if (_entityFrameworkSecondLevelCacheRedisConfiguration is null)
                {
                    throw new InvalidOperationException("No Redis configuration for second level cache was specified");
                }

                _entityFrameworkSecondLevelCacheRedisConfiguration.Apply(config);
            }, "EFSecondLevelRedisCache");
        });
    }

    private void BuildAutoMapper()
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
        {
            return;
        }

        var assemblies = new List<Assembly> { entryAssembly };

        var referencedAssemblies = entryAssembly.GetReferencedAssemblies();

        if (referencedAssemblies is not null)
        {
            foreach (var referencedAssembly in referencedAssemblies)
            {
                var assemblyName = referencedAssembly.Name ?? string.Empty;

                if (assemblyName.EndsWith(".Services", StringComparison.InvariantCulture))
                {
                    var assembly = Assembly.Load(assemblyName);

                    if (assembly is not null)
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        foreach (var assembly in assemblies)
        {
            Services.AddAutoMapper(assembly);
        }
    }

    void IMicroservice.Run()
    {
        throw new NotImplementedException();
    }

    Task IMicroservice.RunAsync()
    {
        throw new NotImplementedException();
    }
#pragma warning restore IDE0058 // Expression value is never used
}
