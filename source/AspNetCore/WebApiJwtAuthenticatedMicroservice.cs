using FFCEI.Microservices.AspNetCore.Jwt;
using FFCEI.Microservices.AspNetCore.Middlewares;
using FFCEI.Microservices.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api microservice template (with JWT Authentication support)
/// </summary>
/// <typeparam name="TWebApiClaims">Web Api Claims for authenticated requests</typeparam>
public class WebApiJwtAuthenticatedMicroservice<TWebApiClaims> : WebApiMicroservice
    where TWebApiClaims : WebApiClaims
{
    /// <summary>
    /// Javascript Web Token: token authority
    /// </summary>
    public string? JwtTokenAuthority { get; set; }

    /// <summary>
    /// Javascript Web Token: validate token issuer
    /// </summary>
    public bool JwtValidateIssuer { get; set; }

    /// <summary>
    /// Javascript Web Token: valid token issuer list
    /// </summary>
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> JwtValidIssuers { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

    /// <summary>
    /// Javascript Web Token: validate token issuer signing key if using and x.509 certificate for signing
    /// </summary>
    public bool JwtValidateIssuerSigningKey { get; set; } = true;

    /// <summary>
    /// Javascript Web Token: validate audience
    /// </summary>
    public bool JwtValidateAudience { get; set; }

    /// <summary>
    /// Javascript Web Token: valid token audience list
    /// </summary>
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> JwtValidAudiences { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

    /// <summary>
    /// Javascript Web Token: validate token lifetime
    /// </summary>
    public bool JwtValidateLifetime { get; set; } = true;

    /// <summary>
    /// Javascript Web Token: clock skew in seconds to validate token lifetime
    /// </summary>
    public int JwtLifetimeClockSkewSeconds { get; set; } = 120;

    /// <summary>
    /// Javascript Web Token: save sign-in web token
    /// </summary>
    public bool JwtSaveSigninToken { get; set; } = true;

    /// <summary>
    /// Javascript Web Token: require HTTPS for web token
    /// </summary>
    public bool JwtRequireHttpsMetadata { get; set; }

    /// <summary>
    /// Javascript Web Token: break debugger on validation errror
    /// </summary>
    public bool JwtDebugBreakOnValidationError { get; set; }

    /// <summary>
    /// Javascript Web Token: late authorization delegate for validating claims or anything you needed after Jwt validation
    /// </summary>
    public JwtPostAuthorizationDelegateMethod? JwtPostAuthorization { get; set; }

#pragma warning disable CA1000
    /// <summary>
    /// Microservice instance (singleton)
    /// </summary>
    public static new WebApiJwtAuthenticatedMicroservice<TWebApiClaims>? Instance => WebApiMicroservice.Instance as WebApiJwtAuthenticatedMicroservice<TWebApiClaims>;
#pragma warning restore CA1000

    /// <summary>
    /// Construct microservice instance
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public WebApiJwtAuthenticatedMicroservice(string[] args)
        : base(args)
    {
        WebApiUseAuthorization = true;
    }

    protected override void OnBuildMicroservice()
    {
        BuildJwtAuthenticator();

        base.OnBuildMicroservice();

        BuildJwtPostAuthorizator();
    }

    protected override void OnCreateMicroservice()
    {
        base.OnCreateMicroservice();

        CreateJwtPostAuthorizator();
    }

#pragma warning disable IDE0058 // Expression value is never used
    private void BuildJwtAuthenticator()
    {
        var signingKeyFactory = new JwtSigningKeyFactory(this.ConfigurationManager);
        var encryptionKeyFactory = new JwtEncryptionKeyFactory(this.ConfigurationManager);
        var argon2KeyFactory = new Argon2KeyFactory(this.ConfigurationManager);

        Services.AddSingleton(signingKeyFactory);
        Services.AddSingleton(encryptionKeyFactory);
        Services.AddSingleton(argon2KeyFactory);

        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
        {
            jwt.Authority = JwtTokenAuthority;
            jwt.SaveToken = JwtSaveSigninToken;
            jwt.IncludeErrorDetails = IsDebugOrDevelopmentEnvironment;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromSeconds(JwtLifetimeClockSkewSeconds),
                RequireSignedTokens = true,
                IssuerSigningKey = signingKeyFactory.SecurityKey,
                ValidateIssuer = JwtValidateIssuer && (JwtValidIssuers.Count > 0),
                ValidIssuers = JwtValidateIssuer && (JwtValidIssuers.Count > 0) ? JwtValidIssuers : null,
                ValidateIssuerSigningKey = JwtValidateIssuer && (JwtValidIssuers.Count > 0) && JwtValidateIssuerSigningKey && signingKeyFactory.SecurityKeyIsX509,
                RequireAudience = JwtValidateAudience && (JwtValidAudiences.Count > 0),
                ValidateAudience = JwtValidateAudience && (JwtValidAudiences.Count > 0),
                ValidAudiences = JwtValidateAudience && (JwtValidAudiences.Count > 0) ? JwtValidAudiences : null,
                ValidateLifetime = JwtValidateLifetime,
                ValidateTokenReplay = JwtValidateLifetime,
                RequireExpirationTime = JwtValidateLifetime,
                TokenDecryptionKey = encryptionKeyFactory.SecurityKey,
                SaveSigninToken = JwtSaveSigninToken,
            };

            if (JwtDebugBreakOnValidationError)
            {
                jwt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = (context) =>
                    {
                        var exceptionMessage = context.Exception;

                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Break();
                        }

                        return Task.CompletedTask;
                    },
                };
            }
        });
    }

    private void BuildJwtPostAuthorizator()
    {
        if (WebApiUseAuthorization)
        {
            Services.AddJwtPostAuthorization(options =>
            {
                options.JwtPostAuthorization = JwtPostAuthorization;
            });
        }
    }
#pragma warning restore IDE0058 // Expression value is never used

    private void CreateJwtPostAuthorizator()
    {
        if (WebApiUseAuthorization)
        {
#pragma warning disable IDE0058 // Expression value is never used
            Application.UseJwtPostAuthorization();
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
