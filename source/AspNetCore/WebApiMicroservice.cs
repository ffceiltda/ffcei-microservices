using Destructurama;
using EFCoreSecondLevelCacheInterceptor;
using FFCEI.Microservices.AspNetCore.Middlewares;
using FFCEI.Microservices.AspNetCore.StaticFolderMappings;
using FFCEI.Microservices.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api microservice template
    /// </summary>
    public class WebApiMicroservice
    {
        private readonly string[] _args;
        private WebApplicationBuilder? _initialBuilder;
        private WebApplicationBuilder? _builder;
        private WebApplication? _application;
        private bool _controllersMapped;
        private ConfigurationManager? _configurationManager;
        private RedisConnectionConfiguration? _entityFrameworkSecondLevelCacheRedisConfiguration;
        private static WeakReference<WebApiMicroservice> _instance = null!;

        /// <summary>
        /// ASP.NET Core Mvc Web Application Builder
        /// </summary>
        public WebApplicationBuilder Builder => _builder ??= CreateBuilder();

        /// <summary>
        /// ASP.NET Core Mvc Web Application
        /// </summary>
        public WebApplication Application => _application ??= CreateApplication();

        /// <summary>
        /// Configuration Manager (with support for system environment, environment files and ASP.NET Core appSettings)
        /// </summary>
        public ConfigurationManager ConfigurationManager => _configurationManager ??= CreateConfigurationManager();

        /// <summary>
        /// Set shutting down to true make all new requests to reply with HTTP Code 503 - Service unavailable, for load balancers
        /// </summary>
        public bool ShuttingDown { get; set; }

        /// <summary>
        /// HTTP settings: Web Api use CORS (defaults to true)
        /// </summary>
        public bool HttpUseCors { get; set; } = true;

        /// <summary>
        /// HTTP settings: Web Api use HSTS (defaults to false )
        /// </summary>
        public bool HttpUseHsts { get; set; }

        /// <summary>
        /// HTTP settings: Web Api redirect to HTTPS (defaults to false)
        /// </summary>
        public bool HttpRedirectToHttps { get; set; }

        /// <summary>
        /// HTTP settings: Request max body length (defaults to 8 hexabytes)
        /// </summary>
        public long HttpRequestMaxBodyLength { get; set; } = long.MaxValue;

        /// <summary>
        /// Web Api: version (defaults to v1)
        /// </summary>
        public string WebApiVersion { get; set; } = "v1";

        /// <summary>
        /// Web Api: generate swagger documentation (defaults to Development environment only)
        /// </summary>
        public bool? WebApiGenerateSwagger { get; set; }

        /// <summary>
        /// Web Api: ignore null values on Json serialization (default to true)
        /// </summary>
        public bool WebApiIgnoreNullsOnJsonSerialization { get; set; } = true;

        /// <summary>
        /// Web Api: require authorization on controller and methods (defaults to false on web api microservices, true on web api jwt authenticated microservices)
        /// </summary>
        public bool WebApiUseAuthorization { get; set; } = true;

        /// <summary>
        /// Web Api: require authorization by default, defaults to false
        /// </summary>
        public bool WebApiUseAuthorizationByDefault { get; set; }

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

        public static WebApiMicroservice? Instance
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
        public WebApiMicroservice(string[] args)
        {
            if (_instance is not null)
            {
                throw new InvalidOperationException("WebApiMicroservice must be instantiated only once");
            }

            _args = args;
            _instance = new WeakReference<WebApiMicroservice>(this);
            _initialBuilder = WebApplication.CreateBuilder(_args);
        }

        private WebApplicationBuilder CreateBuilder()
        {
            if ((_builder is not null) || (_initialBuilder is null))
            {
                throw new InvalidOperationException("WebApiMicroservice CreateBuilder() was already called before");
            }

            _builder = _initialBuilder;
            _initialBuilder = null;

            OnCreateBuilder();

            return _builder;
        }

        /// <summary>
        /// Create Web Application builder settings
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws when builder is null</exception>
        protected virtual void OnCreateBuilder()
        {
            BuildSerilog();
            BuildKestrel();
            BuildWebApi();
            BuildEntityFramework();
            BuildAutoMapper();
        }

        private WebApplication CreateApplication()
        {
            if (_application is not null)
            {
                throw new InvalidOperationException("WebApiMicroservice CreateApplication() was already called before");
            }

            _application = Builder.Build();

#pragma warning disable IDE0058 // Expression value is never used
            Application.UseShuttingDownHandler();
#pragma warning restore IDE0058 // Expression value is never used

            OnCreateApplication();

            return _application;
        }

        /// <summary>
        /// Create Web Application settings
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws when webApplication is null</exception>
        protected virtual void OnCreateApplication()
        {
            CreateSerilog();
            CreateKestrel();
            CreateWebApi();
        }

        private ConfigurationManager CreateConfigurationManager()
        {
            var builder = _builder ?? _initialBuilder;

            if (builder is null)
            {
                throw new InvalidOperationException("WebApiMicroservice CreateConfigurationManager() internal error");
            }

            _configurationManager = new ConfigurationManager(builder);

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
                throw new InvalidOperationException("UseRedisEntityFrameworkSecondLevelCache must be used before access WebApiMicroservice.Builder property");
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
                throw new InvalidOperationException("UseRedisEntityFrameworkSecondLevelCache must be used before access WebApiMicroservice.Builder property");
            }

            if (configuration is null)
            {
                var standardConfiguration = ConfigurationManager.GetRedisConfiguration(
                    hostSettingName: "Redis.Cache.Host",
                    portSettingName: "Redis.Cache.Port",
                    usernameSettingName: "Redis.Cache.Username",
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
        /// Map Controolers
        /// </summary>
        public void MapControllers()
        {
            if (_controllersMapped)
            {
                return;
            }

#pragma warning disable IDE0058 // Expression value is never used
            Application.MapControllers();
#pragma warning restore IDE0058 // Expression value is never used

            _controllersMapped = true;
        }

#pragma warning disable CA1054 // URI-like parameters should not be strings
        /// <summary>
        /// Run microservice
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
        public void Run(string? url = null)
        {
            MapControllers();

            Application.Run(url);
        }

        /// <summary>
        /// Run microservice asynchronously
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
        public async Task RunAsync(string? url = null)
        {
            MapControllers();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await Application.RunAsync(url);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
#pragma warning restore CA1054 // URI-like parameters should not be strings

#pragma warning disable IDE0058 // Expression value is never used
        private bool ShouldGenerateSwagger()
        {
            bool generateSwagger = WebApiGenerateSwagger ?? false;

            if (Builder.Environment.IsDevelopment())
            {
                generateSwagger = WebApiGenerateSwagger ?? true;
            }

            return generateSwagger;
        }

        private void BuildSerilog()
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            Log.Logger = BuildSeriLogConfiguration(Builder, new LoggerConfiguration()).CreateBootstrapLogger();

            Builder.Host.UseSerilog((context, serviceProvider, configuration) => BuildSeriLogConfiguration(Builder, configuration));
        }

        private static LoggerConfiguration BuildSeriLogConfiguration(WebApplicationBuilder builder, LoggerConfiguration configuration)
        {
            configuration
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationIdHeader()
                .Destructure.UsingAttributes()
                .WriteTo.Console();

            if (builder.Environment.IsProduction())
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

        private void CreateSerilog()
        {
            Application.UseSerilogRequestLogging();
        }

        private void BuildKestrel()
        {
            if (HttpUseCors)
            {
                Builder.Services.AddCors(options => options
                    .AddDefaultPolicy(policy => CreateKestrelCorsPolicy(policy)));
            }

            Builder.Host.ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.Limits.MaxRequestBodySize = HttpRequestMaxBodyLength;
                });
            });

            Builder.Services.AddHttpContextAccessor();
        }

        private void CreateKestrel()
        {
            if (HttpUseCors)
            {
                Application.UseCors(policy => CreateKestrelCorsPolicy(policy));
            }

            if (HttpUseHsts)
            {
                Application.UseHsts();
            }

            if (HttpRedirectToHttps)
            {
                Application.UseHttpsRedirection();
            }
        }

        private static CorsPolicyBuilder CreateKestrelCorsPolicy(CorsPolicyBuilder policy)
        {
            return policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }

        private void BuildWebApi()
        {
            Builder.Services.AddHttpClient();

            if (!Builder.Environment.IsDevelopment())
            {
                Builder.Services.AddTransient<ExceptionHandlerMiddeware>();
            }

            if (WebApiUseAuthorization)
            {
                Builder.Services.AddAuthorization(options =>
                {
                    if (WebApiUseAuthorizationByDefault)
                    {
                        options.FallbackPolicy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                    }
                });
            }

            var mvcBuilder = Builder.Services.AddControllers();

            BuildWebApiJsonOptions(mvcBuilder);
            BuildWebApiFluentValidation(mvcBuilder);

            if (ShouldGenerateSwagger())
            {
                BuildWebApiSwagger();
            }
        }

        private void BuildWebApiJsonOptions(IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new Json.JsonTrimmingConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.JsonLooseStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.JsonStringToDecimalConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.JsonStringToLongConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.JsonStringToIntegerConverter());

                if (WebApiIgnoreNullsOnJsonSerialization)
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
            });
        }

        private void BuildWebApiFluentValidation(IMvcBuilder mvcBuilder)
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

                    if (assemblyName.EndsWith(".Messages", StringComparison.InvariantCulture))
                    {
                        var assembly = Assembly.Load(assemblyName);

                        if (assembly is not null)
                        {
                            assemblies.Add(assembly);
                        }
                    }
                }
            }

            mvcBuilder.AddFluentValidation(validators =>
            {
                validators.RegisterValidatorsFromAssemblies(assemblies);
            });

            Builder.Services.AddFluentValidationAutoValidation();
        }

        private void BuildWebApiSwagger()
        {
            Builder.Services.AddEndpointsApiExplorer();
            Builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(WebApiVersion,
                    new OpenApiInfo
                    {
                        Title = Builder.Environment.ApplicationName,
                        Version = WebApiVersion
                    });

                options.TagActionsBy(api =>
                {
                    if (api.GroupName is not null)
                    {
                        return new[] { api.GroupName };
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    return new[] { string.Empty };
                });

                options.DocInclusionPredicate((docName, apiDesc) => true);

                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nPlease, enter 'Bearer'[space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"\r\n\r\n",
                        In = ParameterLocation.Header
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();

                xmlFiles.ForEach(xmlFile =>
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
                        options.IncludeXmlComments(xmlFile);

                        options.SchemaFilter<Swagger.EnumTypesSchemaFilter>(xmlFile);
                        options.SchemaFilter<Swagger.MessageAttributeSchemaFilter>(xmlFile);
                        options.DocumentFilter<Swagger.PropertyAttributeDocumentFilter>();
                        options.OperationFilter<SecurityRequirementsOperationFilter>(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception while enriching Swagger documentation: " + ex.Message);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                });
            });
        }

        private void CreateWebApi()
        {
            if (Application.Environment.IsDevelopment())
            {
                Application.UseDeveloperExceptionPage();
            }
            else
            {
                Application.UseExceptionHandler(a => a.Run(async context =>
                {
                    var exceptionHandlerMiddeware = Application.Services.GetService<ExceptionHandlerMiddeware>();

                    if (exceptionHandlerMiddeware is not null)
                    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                        await exceptionHandlerMiddeware.InvokeAsync(context);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
                    }
                }));
            }

            CreateWebApiSwagger();

            Application.UseRouting();

            CreateWebApiStaticFolderMappings();
            CreateWebApiAuthenticationAndAuthorization();
        }

        private void CreateWebApiSwagger()
        {
            if (ShouldGenerateSwagger())
            {
                Application.UseSwagger();
                Application.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint($"/swagger/{WebApiVersion}/swagger.json", $"{Application.Environment.ApplicationName} ({WebApiVersion})");
                });
            }
        }

        private void CreateWebApiStaticFolderMappings()
        {
            var configuration = Application.Services.GetService<IConfigureOptions<StaticFolderMappingMiddlewareOptions>>();

            if (configuration == null)
            {
                return;
            }

            var options = new StaticFolderMappingMiddlewareOptions();

            configuration.Configure(options);

            if (options.Empty)
            {
                return;
            }

            Application.UseStaticFolderMappings();

            foreach (var mapping in options.MappedFolders.Values)
            {
                var requestPath = mapping.WebPath[..^1];

                Application.UseStaticFiles(options: new StaticFileOptions()
                {
                    RequestPath = requestPath,
                    FileProvider = new PhysicalFileProvider(mapping.PhysicalPath),
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/octet-stream"
                });

                if (mapping.DirectoryBrowsing)
                {
                    Application.UseDirectoryBrowser(options: new DirectoryBrowserOptions()
                    {
                        RequestPath = requestPath,
                        FileProvider = new PhysicalFileProvider(mapping.PhysicalPath)
                    });
                }
            }
        }

        private void CreateWebApiAuthenticationAndAuthorization()
        {
            Application.UseAuthentication();

            if (WebApiUseAuthorization)
            {
                Application.UseAuthorization();
            }
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
                .DisableLogging(!Builder.Environment.IsDevelopment());
        }

        private void BuildMemoryEntityFrameworkCoreSecondLevelCache()
        {
            Builder.Services.AddEFSecondLevelCache(options =>
            {
                BuildEntityFrameworkCoreSecondLevelCacheOptions(options.UseMemoryCacheProvider());
            });
        }

        private void BuildRedisEntityFrameworkCoreSecondLevelCache()
        {
            Builder.Services.AddEFSecondLevelCache(options =>
            {
                BuildEntityFrameworkCoreSecondLevelCacheOptions(options.UseEasyCachingCoreProvider("EFSecondLevelRedisCache"));
            });

            Builder.Services.AddEasyCaching(option =>
            {
                option.WithJson();
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
                Builder.Services.AddAutoMapper(assembly);
            }
        }
#pragma warning restore IDE0058 // Expression value is never used
    }
}
