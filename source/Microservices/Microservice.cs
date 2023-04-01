using Destructurama;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace FFCEI.Microservices.Microservices;

/// <summary>
/// Microservice base implementation
/// </summary>
public abstract class Microservice : IMicroservice
{
    private IHostBuilder? _builder;
    private IHostEnvironment? _environment;
    private IServiceCollection? _services;
    private IHost? _host;
    private IConfigurationManager? _configurationManager;
    private RedisConnectionConfiguration? _entityFrameworkSecondLevelCacheRedisConfiguration;
    private static WeakReference<Microservice> _instance = null!;

    /// <summary>
    /// Registry path for environment settings search in HKCU, then HKLM
    /// </summary>
    public static string? RegistryPathForEnvironment { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="commandLineArguments">Program command line arguments</param>
    protected Microservice(string[] commandLineArguments)
    {
        _instance = new WeakReference<Microservice>(this);

        CommandLineArguments = commandLineArguments.ToList();

        IsDebugOrDevelopment = Debugger.IsAttached || string.Equals(System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? string.Empty, "development", StringComparison.OrdinalIgnoreCase);

        Logger = CreateMicroserviceBootstrapLogger();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
        Logger.LogInformation("Microservice {MicroserviceName} starting at {DateTimeOffsetNow} (in Debug or Development mode: {IsDebugOrDevelopment})",
            MicroserviceName, DateTimeOffset.Now, IsDebugOrDevelopment);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
    }

    public string MicroserviceName { get; protected set; } = Process.GetCurrentProcess().ProcessName;

    public IReadOnlyList<string> CommandLineArguments { get; private set; }

    public IHostBuilder Builder => _builder ??= CreateBuilder();

    public IHostEnvironment Environment => _environment ??= GetImplementationEnvironment();

    public IServiceCollection Services => _services ??= GetImplementationServices();

    public IHost Host => _host ??= BuildHost();

    public IConfigurationManager ConfigurationManager => _configurationManager ??= CreateConfigurationManager();

    public ILogger Logger { get; private set; }

    public bool IsDebugOrDevelopment { get; protected set; }

#pragma warning disable CA1000
    /// <summary>
    /// Microservice instance (singleton)
    /// </summary>
    public static Microservice? Instance
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
#pragma warning restore CA1000

    /// <summary>
    /// Variable to control shutdown request
    /// </summary>
    public bool ShuttingDown { get; set; }

    /// <summary>
    /// Json: ignore null values on serialization (default to true)
    /// </summary>
    public bool RedisCacheJsonIgnoreNullOnSerialization { get; set; } = true;

    /// <summary>
    /// Json: write indented on serialization (default to false)
    /// </summary>
    public bool RedisCacheJsonWriteIndented { get; set; }

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
    /// Load referenced assemblies ending with text
    /// </summary>
    /// <param name="endingText">End text</param>
    /// <returns>List of matching loaded assemblies</returns>
    internal static List<Assembly> ReferencedAssembliesEndingWith(string endingText)
    {
        var assemblies = new List<Assembly>();
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is not null)
        {
            assemblies.Add(entryAssembly);

            var referencedAssemblies = entryAssembly.GetReferencedAssemblies();

            if (referencedAssemblies is not null)
            {
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    var assemblyName = referencedAssembly.Name ?? string.Empty;

                    if (assemblyName.EndsWith(endingText, StringComparison.InvariantCulture))
                    {
                        var assembly = Assembly.Load(assemblyName);

                        if (assembly is not null)
                        {
                            assemblies.Add(assembly);
                        }
                    }
                }
            }
        }

        return assemblies;
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
    /// Return initial IHostBuilder for early configuration
    /// </summary>
    /// <returns>IHostBuilder instance</returns>
    protected abstract IHostBuilder? GetImplementationInitialBuilder();

    private IHostBuilder CreateBuilder()
    {
        if (_builder is not null)
        {
            throw new InvalidOperationException("Microservice CreateBuilder() was already called before");
        }

        var initialBuilder = GetImplementationInitialBuilder();

        if (initialBuilder is null)
        {
            throw new InvalidOperationException("Microservice CreateBuilder() logic error");
        }

        _builder = initialBuilder;

        OnBuildMicroservice();

        return _builder;
    }

    /// <summary>
    /// Retuirn IServiceCollection for configuration
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">throws If cannot get ServiceCollection instance</exception>
    protected abstract IServiceCollection GetImplementationServices();

    private IConfigurationManager CreateConfigurationManager()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Builder.ConfigureServices((context, services) =>
        {
            _configurationManager = new ConfigurationManager(Logger, Builder, context.Configuration);
        });
#pragma warning restore IDE0058 // Expression value is never used

        if (_configurationManager is null)
        {
            throw new InvalidOperationException("ConfigurationManager cannot be instantiated");
        }

        return _configurationManager;
    }

    /// <summary>
    /// Return current IHostEnvironment for application
    /// </summary>
    /// <returns></returns>
    protected abstract IHostEnvironment GetImplementationEnvironment();

    /// <summary>
    /// Return IHost for application startup
    /// </summary>
    /// <returns></returns>
    protected abstract IHost GetImplementationHost();

    private ILogger CreateMicroserviceBootstrapLogger()
    {
        SelfLog.Enable(Console.Error);

        var bootstrapLogger = BuildSeriLogConfiguration(new LoggerConfiguration()).CreateBootstrapLogger();

        Log.Logger = bootstrapLogger;

        using var logProvider = new SerilogLoggerProvider(bootstrapLogger, false);

        return logProvider.CreateLogger("Microservice Bootstrapper");
    }

    protected virtual LoggerConfiguration BuildSeriLogConfiguration(LoggerConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
        configuration
            .Enrich.WithProperty("Application", MicroserviceName)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationIdHeader()
            .Destructure.UsingAttributes()
            .WriteTo.Console();
#pragma warning restore CA1305 // Specify IFormatProvider

        if (!IsDebugOrDevelopment)
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
#pragma warning restore IDE0058 // Expression value is never used

        return configuration;
    }

    protected virtual void ConfigureJsonSerializerOptions(JsonSerializerOptions options) =>
        options.ConfigureJsonSerializerOptions(RedisCacheJsonWriteIndented, RedisCacheJsonIgnoreNullOnSerialization);

    private IHost BuildHost()
    {
        if (_host is not null)
        {
            throw new InvalidOperationException("Microservice CreateApplication() was already called before");
        }

        var builder = Builder;

        if (builder is null)
        {
            throw new InvalidOperationException("Microservice CreateApplication() logic error");
        }

        _host = GetImplementationHost();

        OnCreateMicroservice();

        return _host;
    }

    /// <summary>
    /// Construct Microservice builder
    /// </summary>
    protected virtual void OnBuildMicroservice()
    {
        BuildSerilog();
        BuildEntityFramework();
        BuildAutoMapper();
    }

    /// <summary>
    /// Build Microservice application
    /// </summary>
    protected virtual void OnCreateMicroservice()
    {
    }

    private void BuildSerilog()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Builder.UseSerilog((context, serviceProvider, configuration) => BuildSeriLogConfiguration(configuration));
#pragma warning restore IDE0058 // Expression value is never used
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

#pragma warning disable IDE0058 // Expression value is never used
        options
            .UseCacheKeyPrefix($"EFCoreSecondLevelCache_{assemblyPrefix}_")
            .CacheAllQueries(EntityFrameworkSecondLevelCacheExpirationMode, EntityFrameworkSecondLevelCacheExpirationPeriod)
            .SkipCachingResults(result => (result.Value is null) || ((result.Value is EFTableRows rows) && (rows.RowsCount == 0)))
            .DisableLogging(!IsDebugOrDevelopment);
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void BuildMemoryEntityFrameworkCoreSecondLevelCache()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Services.AddEFSecondLevelCache(options =>
        {
            BuildEntityFrameworkCoreSecondLevelCacheOptions(options.UseMemoryCacheProvider());
        });
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void BuildRedisEntityFrameworkCoreSecondLevelCache()
    {
#pragma warning disable IDE0058 // Expression value is never used
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
#pragma warning restore IDE0058 // Expression value is never used
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
#pragma warning disable IDE0058 // Expression value is never used
            Services.AddAutoMapper(assembly);
#pragma warning restore IDE0058 // Expression value is never used
        }
    }

    public virtual void Run()
    {
        Host.Run();
    }

    public virtual async Task RunAsync()
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await Host.RunAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
