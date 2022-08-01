using FFCEI.Microservices.AspNetCore.Middlewares;
using FFCEI.Microservices.AspNetCore.StaticFolderMappings;
using FFCEI.Microservices.Microservices;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api microservice template
/// </summary>
public class WebApiMicroservice : Microservice
{
    private IHostBuilder? _initialBuilder;
    private WebApplicationBuilder? _applicationBuilder;
    private WebApplication? _application;
    private bool _controllersMapped;

    /// <summary>
    /// ASP.NET Web Application
    /// </summary>
    public WebApplication Application => _application ??= CreateWebApplication();

    /// <summary>
    /// HTTP settings: Web Api use CORS (defaults to true)
    /// </summary>
    public bool HttpUseCors { get; set; } = true;

    /// <summary>
    /// HTTP settings: Request max body length (defaults to 8 hexabytes)
    /// </summary>
    public long HttpRequestMaxBodyLength { get; set; } = long.MaxValue;

    /// <summary>
    /// HTTP settings: Web Api redirect to HTTPS (defaults to false)
    /// </summary>
    public bool HttpRedirectToHttps { get; set; }

    /// <summary>
    /// HTTP settings: Web Api use HSTS (defaults to false )
    /// </summary>
    public bool HttpsUseHsts { get; set; }

    /// <summary>
    /// Web Api: generate swagger documentation (defaults to Development environment only)
    /// </summary>
    public bool? WebApiGenerateSwagger { get; set; }

    /// <summary>
    /// Web Api: version (defaults to v1)
    /// </summary>
    public string WebApiSwaggerVersion { get; set; } = "v1";

    /// <summary>
    /// Web Api: require authorization on controller and methods (defaults to false on web api microservices, true on web api jwt authenticated microservices)
    /// </summary>
    public bool WebApiUseAuthorization { get; set; } = true;

    /// <summary>
    /// Web Api: require authorization by default, defaults to false
    /// </summary>
    public bool WebApiUseAuthorizationByDefault { get; set; }

    /// <summary>
    /// Json for Web Api: ignore null values on serialization (default to true)
    /// </summary>
    public bool WebApiJsonIgnoreNullOnSerialization { get; set; } = true;

    /// <summary>
    /// Json for Web Api: write indented on serialization (default to true)
    /// </summary>
    public bool WebApiJsonWriteIndented { get; set; } = true;

#pragma warning disable CA1000
    /// <summary>
    /// Microservice instance (singleton)
    /// </summary>
    public static new WebApiMicroservice? Instance => Microservice.Instance as WebApiMicroservice;
#pragma warning restore CA1000

    /// <summary>
    /// Web Api Microservice constructor
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments</param>
    public WebApiMicroservice(string[] commandLineArguments)
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

        if (builder is null)
        {
            throw new InvalidOperationException("Web Api Microservice CreateWebApplication() logic error");
        }

        _application = builder.Build();

        return _application;
    }

    protected override IHost GetImplementationApplication() => Application;

    protected override void OnBuildMicroservice()
    {
        base.OnBuildMicroservice();

        BuildKestrel();
        BuildWebApi();
    }

    protected override void OnCreateMicroservice()
    {
        base.OnCreateMicroservice();

#pragma warning disable IDE0058 // Expression value is never used
        Application.UseShuttingDownHandler();
#pragma warning restore IDE0058 // Expression value is never used

        CreateSerilog();
        CreateKestrel();
        CreateWebApi();
    }

    /// <summary>
    /// Map Asp.Net Controllers
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

    public override void Run()
    {
        MapControllers();

        base.Run();
    }

    public override async Task RunAsync()
    {
        MapControllers();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await base.RunAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }

#pragma warning disable CA1054 // URI-like parameters should not be strings
    /// <summary>
    /// Run Asp.Net microservice
    /// </summary>
    /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
    public void Run(string? url)
    {
        MapControllers();

        Application.Run(url);
    }

    /// <summary>
    /// Run Asp.Net microservice asynchronously
    /// </summary>
    /// <param name="url">The URL to listen to if the server hasn't been configured directly</param>
    public async Task RunAsync(string? url)
    {
        MapControllers();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await Application.RunAsync(url);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
#pragma warning restore CA1054 // URI-like parameters should not be strings

    private static CorsPolicyBuilder CreateKestrelCorsPolicy(CorsPolicyBuilder policy)
    {
        return policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }

    private void CreateSerilog()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Application.UseSerilogRequestLogging();
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void BuildKestrel()
    {
#pragma warning disable IDE0058 // Expression value is never used
        if (HttpUseCors)
        {
            Services.AddCors(options => options.AddDefaultPolicy(policy => CreateKestrelCorsPolicy(policy)));
        }

        Builder.ConfigureServices((context, services) =>
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = HttpRequestMaxBodyLength;
            });
        });

        Services.AddHttpContextAccessor();
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void CreateKestrel()
    {
#pragma warning disable IDE0058 // Expression value is never used
        if (HttpUseCors)
        {
            Application.UseCors(policy => CreateKestrelCorsPolicy(policy));
        }

        if (HttpRedirectToHttps)
        {
            Application.UseHttpsRedirection();
        }

        if (HttpsUseHsts)
        {
            Application.UseHsts();
        }
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void BuildWebApi()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Services.AddHttpClient();

        if (!IsDebugOrDevelopment)
        {
            Services.AddTransient<ExceptionHandlerMiddeware>();
        }

        if (WebApiUseAuthorization)
        {
            Services.AddAuthorization(options =>
            {
                if (WebApiUseAuthorizationByDefault)
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                }
            });
        }
#pragma warning restore IDE0058 // Expression value is never used

        var mvcBuilder = Services.AddControllers();

        BuildWebApiJsonOptions(mvcBuilder);
        BuildWebApiFluentValidation(mvcBuilder);

        if (ShouldGenerateSwagger())
        {
            BuildWebApiSwagger();
        }
    }

    private void ConfigureJsonSerializerOptions(JsonSerializerOptions options, bool isWebApi)
    {
        base.ConfigureJsonSerializerOptions(options);

        options.WriteIndented = !isWebApi ? options.WriteIndented : WebApiJsonWriteIndented;
        options.DefaultIgnoreCondition = !isWebApi ? options.DefaultIgnoreCondition : (WebApiJsonIgnoreNullOnSerialization ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never);
    }

    private void BuildWebApiJsonOptions(IMvcBuilder mvcBuilder)
    {
#pragma warning disable IDE0058 // Expression value is never used
        mvcBuilder.AddJsonOptions(options =>
        {
            ConfigureJsonSerializerOptions(options.JsonSerializerOptions, true);
        });
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void BuildWebApiFluentValidation(IMvcBuilder mvcBuilder)
    {
        var assemblies = ReferencedAssembliesEndingWith(".Messages");

#pragma warning disable IDE0058 // Expression value is never used
        mvcBuilder.AddFluentValidation(validators =>
        {
            validators.RegisterValidatorsFromAssemblies(assemblies);
        });

        Services.AddFluentValidationAutoValidation();
#pragma warning restore IDE0058 // Expression value is never used
    }

    private bool ShouldGenerateSwagger()
    {
        bool generateSwagger = WebApiGenerateSwagger ?? false;

        if (IsDebugOrDevelopment)
        {
            generateSwagger = WebApiGenerateSwagger ?? true;
        }

        return generateSwagger;
    }

    private void BuildWebApiSwagger()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(WebApiSwaggerVersion, new OpenApiInfo
            {
                Title = MicroserviceName,
                Version = WebApiSwaggerVersion
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
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Exception while enriching Swagger documentation");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            });
        });
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void CreateWebApi()
    {
#pragma warning disable IDE0058 // Expression value is never used
        if (IsDebugOrDevelopment)
        {
            Application.UseDeveloperExceptionPage();
        }
        else
        {
            Application.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerMiddeware = Host.Services.GetService<ExceptionHandlerMiddeware>();

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
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void CreateWebApiSwagger()
    {
#pragma warning disable IDE0058 // Expression value is never used
        if (ShouldGenerateSwagger())
        {
            Application.UseSwagger();
            Application.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint($"/swagger/{WebApiSwaggerVersion}/swagger.json", $"{MicroserviceName} ({WebApiSwaggerVersion})");
            });
        }
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void CreateWebApiStaticFolderMappings()
    {
        var options = Host.Services.GetService<StaticFolderMappingMiddlewareOptions>();

        if (options is null)
        {
            return;
        }

#pragma warning disable IDE0058 // Expression value is never used
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
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void CreateWebApiAuthenticationAndAuthorization()
    {
#pragma warning disable IDE0058 // Expression value is never used
        Application.UseAuthentication();

        if (WebApiUseAuthorization)
        {
            Application.UseAuthorization();
        }
#pragma warning restore IDE0058 // Expression value is never used
    }
}
