using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FFCEI.Microservices.AspNetCore
{
    public class WebApiJwtAuthenticatedMicroservice<TWebApiClaims> : WebApiMicroservice
        where TWebApiClaims : WebApiClaims
    {
#pragma warning disable CA1000
        public static new WebApiJwtAuthenticatedMicroservice<TWebApiClaims>? Instance =>
            WebApiMicroservice.Instance as WebApiJwtAuthenticatedMicroservice<TWebApiClaims>;
#pragma warning restore CA1000

        public WebApiJwtAuthenticatedMicroservice(string[] args)
            : base(args)
        {
        }

        public override WebApplicationBuilder CreateBuilder()
        {
            var builder = base.CreateBuilder();

            BuildJwtAuthenticator(builder);

            return builder;
        }

        public override WebApplication CreateApplication()
        {
            var application = base.CreateApplication();

            return application;
        }

#pragma warning disable IDE0058 // Expression value is never used
        private void BuildJwtAuthenticator(WebApplicationBuilder builder)
        {
            var tokenSecret = new Jwt.TokenSecret(ConfigurationManager);

            builder.Services.AddSingleton(tokenSecret);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(tokenSecret.KeyBytes),
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateTokenReplay = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
#pragma warning restore IDE0058 // Expression value is never used
    }
}
