using Destructurama;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api microservice template
    /// </summary>
    public class WebApiMicroservice
    {
        private readonly string[] _args;
        private WebApplicationBuilder? _builder;
        private WebApplication? _application;
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
        public FFCEI.Microservices.Configuration.ConfigurationManager ConfigurationManager { get; private set; } = null!;

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
        /// Microservice instance
        /// </summary>

        public static WebApiMicroservice? Instance
        {
            get
            {
                if ((_instance != null) && _instance.TryGetTarget(out var result))
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
            if (_instance != null)
            {
                throw new InvalidOperationException("WebApiMicroservice must be instantiated only once");
            }

            _args = args;
            _instance = new WeakReference<WebApiMicroservice>(this);
        }

        /// <summary>
        /// Create Web Application Builder and apply internal builder settings
        /// </summary>
        /// <returns>WebApplicationBuilder instance</returns>
        protected virtual WebApplicationBuilder CreateBuilder()
        {
            var builder = WebApplication.CreateBuilder(_args);

            OnCreateBuilder(builder);

            return builder;
        }

        /// <summary>
        /// Create Web Application and apply internal application settings
        /// </summary>
        /// <returns>WebApplication instance</returns>
        protected virtual WebApplication CreateApplication()
        {
            var webApplication = Builder.Build();

            OnCreateApplication(webApplication);

            return webApplication;
        }

        /// <summary>
        /// Create Web Application builder settings
        /// </summary>
        /// <param name="builder">WebApplicationBuilder instance</param>
        /// <exception cref="ArgumentNullException">Throws when builder is null</exception>
        private void OnCreateBuilder(WebApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ConfigurationManager = new FFCEI.Microservices.Configuration.ConfigurationManager(builder);

            BuildSerilog(builder);
            BuildKestrel(builder);
            BuildWebApi(builder);
            BuildAutoMapper(builder);
        }

        /// <summary>
        /// Create Web Application settings
        /// </summary>
        /// <param name="webApplication">WebApplication instance</param>
        /// <exception cref="ArgumentNullException">Throws when webApplication is null</exception>
        public void OnCreateApplication(WebApplication webApplication)
        {
            if (webApplication == null)
            {
                throw new ArgumentNullException(nameof(webApplication));
            }

            CreateSerilog(webApplication);
            CreateKestrel(webApplication);
            CreateWebApi(webApplication);
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
        private static void CreateSerilog(WebApplication webApplication)
        {
            webApplication.UseSerilogRequestLogging();
        }

        private void CreateKestrel(WebApplication webApplication)
        {
            webApplication.UseCors();

            if (HttpRedirectToHttps)
            {
                webApplication.UseHttpsRedirection();
            }
        }

        private void CreateWebApi(WebApplication webApplication)
        {
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseDeveloperExceptionPage();
            }

            if (ShouldGenerateSwagger(Builder))
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint($"/swagger/{WebApiVersion}/swagger.json", $"{webApplication.Environment.ApplicationName} ({WebApiVersion})");
                });
            }

            /*
            webApplication.UseMiddleware<AcceptRequestMidleware>();
            webApplication.UseMiddleware<TokenValidationMiddleware>();
            webApplication.UseMiddleware<AutoRenewTokenMiddleware>();
            webApplication.UseMiddleware<LogRequestAndResponseMiddleware>();
            */

            webApplication.UseRouting();

            webApplication.UseAuthentication();
            webApplication.UseAuthorization();

            webApplication.MapControllers();
        }

        private bool ShouldGenerateSwagger(WebApplicationBuilder builder)
        {
            bool generateSwagger = WebApiGenerateSwagger ?? false;

            if (builder.Environment.IsDevelopment())
            {
                generateSwagger = WebApiGenerateSwagger ?? true;
            }

            return generateSwagger;
        }

        private static void BuildSerilog(WebApplicationBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            Log.Logger = BuildSeriLogConfiguration(builder, new LoggerConfiguration()).CreateBootstrapLogger();

            builder.Host.UseSerilog((context, serviceProvider, configuration) => BuildSeriLogConfiguration(builder, configuration));
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

        private void BuildKestrel(WebApplicationBuilder builder)
        {
            if (HttpUseCors)
            {
                builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()));
            }

            builder.Host.ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.Limits.MaxRequestBodySize = HttpRequestMaxBodyLength;
                });
            });

            builder.Services.AddHttpContextAccessor();
        }

        private void BuildWebApi(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();

            var mvcBuilder = builder.Services.AddControllers();

            BuildWebApiJsonOptions(mvcBuilder);
            BuildWebApiFluentValidation(mvcBuilder);

            if (ShouldGenerateSwagger(builder))
            {
                BuildWebApiSwagger(builder);
            }
        }

        private static void BuildWebApiJsonOptions(IMvcBuilder builder)
        {
            builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new Json.TrimmingConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.LooseStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToDecimalConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToLongConverter());
                options.JsonSerializerOptions.Converters.Add(new Json.StringToIntegerConverter());
            });
        }

        private static void BuildWebApiFluentValidation(IMvcBuilder builder)
        {
            builder.AddFluentValidation(validators =>
            {
                validators.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
            });
        }

        private static void BuildAutoMapper(WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
        }

        private void BuildWebApiSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(WebApiVersion,
                    new OpenApiInfo
                    {
                        Title = Builder.Environment.ApplicationName,
                        Version = WebApiVersion
                    });

                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
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
#pragma warning restore IDE0058 // Expression value is never used
    }
}
