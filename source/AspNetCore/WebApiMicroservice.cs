using Destructurama;
using EFCoreSecondLevelCacheInterceptor;
using FFCEI.Microservices.AspNetCore.StaticFiles;
using FFCEI.Microservices.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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
        private ConfigurationManager? _configurationManager;
        private RedisConnectionConfiguration? _entityFrameworkSecondLevelCacheRedisConfiguration;
        private SortedDictionary<string, FolderMapping> _staticFolderMappings = new();
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
        /// HTTP settings: Web Api use CORS (defaults to true)
        /// </summary>
        public bool HttpUseCors { get; set; } = true;

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
        public EntityFrameworkSecondLevelCache EntityFrameworkSecondLevelCache { get; set; } = EntityFrameworkSecondLevelCache.NoCache;

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

        public void UseMemoryEntityFrameworkSecondLevelCache()
        {
            EntityFrameworkSecondLevelCache = EntityFrameworkSecondLevelCache.MemoryCache;
        }

        public void UseRedisEntityFrameworkSecondLevelCache(RedisConnectionConfiguration? configuration = null)
        {
            if (configuration == null)
            {
                var standardConfiguration = ConfigurationManager.GetRedisConfiguration(
                    hostSettingName: "Redis.Cache.Host",
                    portSettingName: "Redis.Cache.Port",
                    usernameSettingName: "Redis.Cache.Username",
                    passwordSettingName: "Redis.Cache.Password",
                    databaseSettingName: "Redis.Cache.Database");

                if (standardConfiguration.Host is null)
                {
                    throw new ArgumentNullException(nameof(configuration));
                }

                configuration = standardConfiguration;
            }

            EntityFrameworkSecondLevelCache = EntityFrameworkSecondLevelCache.RedisCache;

            _entityFrameworkSecondLevelCacheRedisConfiguration = configuration;
        }

        /// <summary>
        /// Add a Static Folder Mapping, and apply authorization policies
        /// </summary>
        /// <param name="webPath">HTTP path</param>
        /// <param name="physicalPath">Physical operating system path</param>
        /// <param name="directoryBrowsing">Enables directory browsing</param>
        /// <param name="authorizationPolicy">Authorization policy</param>
        /// <param name="authorizedRoles">Authorized roles (if applies)</param>
        /// <exception cref="InvalidOperationException">Throws if webPath is already mapped</exception>
        public void UseStaticFolderMapping(string webPath, string physicalPath, bool directoryBrowsing = false,
            StaticFolderMappingAuthorizationPolicy authorizationPolicy = StaticFolderMappingAuthorizationPolicy.PublicAccess,
            IEnumerable<string>? authorizedRoles = null)
        {
            if (webPath == null)
            {
                throw new ArgumentNullException(nameof(webPath));
            }

            if (physicalPath == null)
            {
                throw new ArgumentNullException(nameof(physicalPath));
            }

            if (!Directory.Exists(physicalPath))
            {
                throw new InvalidOperationException($"Directory {physicalPath} cannot be accessed or does not exists");
            }

            while (webPath.StartsWith("/", StringComparison.InvariantCulture))
            {
                webPath = webPath.Substring(1);
            }

            while (webPath.EndsWith("/", StringComparison.InvariantCulture))
            {
                webPath = webPath.Substring(0, webPath.Length - 1);
            }

            if (_staticFolderMappings.ContainsKey(webPath))
            {
                throw new InvalidOperationException($"WebApi Static Path {webPath} already configured");
            }

            var mapping = new FolderMapping()
            {
                WebPath = $"/{webPath}/",
                PhysicalPath = physicalPath,
                DirectoryBrowsing = directoryBrowsing,
                AuthorizationPolicy = authorizationPolicy, 
                AuthorizedRoles = authorizedRoles?.ToHashSet()
            };

            _staticFolderMappings.Add(webPath, mapping);
        }

#pragma warning disable CA1054 // URI-like parameters should not be strings
        /// <summary>
        /// Run microservice
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
        public void Run(string? url = null)
        {
            Application.Run(url);
        }

        /// <summary>
        /// Run microservice asynchronously
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
        public async Task RunAsync(string? url = null)
        {
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
                Builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()));
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
            Application.UseCors();

            if (HttpRedirectToHttps)
            {
                Application.UseHttpsRedirection();
            }
        }

        private void BuildWebApi()
        {
            Builder.Services.AddHttpClient();

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
                options.JsonSerializerOptions.Converters.Add(new Json.TrimmingConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.LooseStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToDecimalConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToLongConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToIntegerConverter());

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

                options.AddSecurityDefinition("oauth2",
                    new OpenApiSecurityScheme
                    {
                        Description = "Standard Authorization header using the Bearer Scheme. Use the format \"Bearer {token}\"",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
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

            if (ShouldGenerateSwagger())
            {
                Application.UseSwagger();
                Application.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint($"/swagger/{WebApiVersion}/swagger.json", $"{Application.Environment.ApplicationName} ({WebApiVersion})");
                });
            }

            Application.UseRouting();

            CreateWebApiStaticFolderMappings();

            if (WebApiUseAuthorization)
            {
                Application.UseAuthentication();
                Application.UseAuthorization();
            }

            Application.MapControllers();
        }

        private void CreateWebApiStaticFolderMappings()
        {
            if (_staticFolderMappings.Count == 0)
            {
                return;
            }

            Application.UseMiddleware<FolderMappingMiddleware>(_staticFolderMappings);

            foreach (var mapping in _staticFolderMappings.Values)
            {
                var requestPath = mapping.WebPath.Substring(0, mapping.WebPath.Length - 1);

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

        private void BuildEntityFramework()
        {
            BuildEntityFrameworkCoreSecondLevelCache();
        }

        private void BuildEntityFrameworkCoreSecondLevelCache()
        {
            switch (EntityFrameworkSecondLevelCache)
            {
            case EntityFrameworkSecondLevelCache.NoCache:
                {
                    break;
                }
            case EntityFrameworkSecondLevelCache.MemoryCache:
                {
                    BuildMemoryEntityFrameworkCoreSecondLevelCache();

                    break;
                }
            case EntityFrameworkSecondLevelCache.RedisCache:
                {
                    BuildRedisEntityFrameworkCoreSecondLevelCache();

                    break;
                }
            default:
                {
                    throw new InvalidOperationException(nameof(EntityFrameworkSecondLevelCache));
                }
            }
        }

        private void BuildEntityFrameworkCoreSecondLevelCacheOptions(EFCoreSecondLevelCacheOptions options)
        {
            var assemblyPrefix = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0].Replace(".", "", StringComparison.InvariantCulture) ?? string.Empty;

            options
                .UseCacheKeyPrefix($"EFCoreSecondLevelCache_{assemblyPrefix}_")
                .CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(1))
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
