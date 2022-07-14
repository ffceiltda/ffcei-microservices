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
    public class WebApiMicroservice
    {
        private readonly string[] _args;
        private WebApplicationBuilder? _builder;
        private WebApplication? _application;

        public WebApplicationBuilder Builder => _builder ??= CreateBuilder();
        public WebApplication Application => _application ??= CreateApplication();
        public FFCEI.Microservices.Configuration.ConfigurationManager ConfigurationManager { get; private set; } = null!;
        public bool HttpUseCors { get; set; } = true;
        public bool HttpRedirectToHttps { get; set; }
        public long HttpRequestMaxBodyLength { get; set; } = long.MaxValue;
        public string WebApiVersion { get; set; } = "v1";
        public bool? WebApiGenerateSwagger { get; set; }

        private static WeakReference<WebApiMicroservice> _instance = null!;

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

        public WebApiMicroservice(string[] args)
        {
            _args = args;
            _instance = new WeakReference<WebApiMicroservice>(this);
        }

        public virtual WebApplicationBuilder CreateBuilder()
        {
            var builder = WebApplication.CreateBuilder(_args);

            OnCreateBuilder(builder);

            return builder;
        }

        public virtual WebApplication CreateApplication()
        {
            var webApplication = Builder.Build();

            OnCreateApplication(webApplication);

            return webApplication;
        }

        public void OnCreateBuilder(WebApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ConfigurationManager = new FFCEI.Microservices.Configuration.ConfigurationManager(builder);

            BuildSerilog(builder);
            BuildKestrel(builder);
            BuildWebApi(builder);
        }

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
        public void Run(string? url = null)
        {
            Application.Run(url);
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        public async Task RunAsync(string? url = null)
        {
            await Application.RunAsync(url);
        }
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
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
            bool generateSwagger = WebApiGenerateSwagger ?? false;

            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseDeveloperExceptionPage();

                generateSwagger = WebApiGenerateSwagger ?? true;
            }

            if (generateSwagger)
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{webApplication.Environment.ApplicationName} ({WebApiVersion})");
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

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new Json.TrimmingConverter());
                    options.JsonSerializerOptions.Converters.Add(new Json.LooseStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new Json.StringToDecimalConverter());
                    options.JsonSerializerOptions.Converters.Add(new Json.StringToLongConverter());
                    options.JsonSerializerOptions.Converters.Add(new Json.StringToIntegerConverter());
                })
                .AddFluentValidation(validators =>
                {
                    validators.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
                });

            if (builder.Environment.IsDevelopment())
            {
                BuildWebApiSwagger(builder);
            }
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
