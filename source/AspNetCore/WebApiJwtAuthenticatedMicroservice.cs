using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api microservice template (with JWT Authentication support)
    /// </summary>
    /// <typeparam name="TWebApiClaims">Web Api Claims for authenticated requests</typeparam>
    public class WebApiJwtAuthenticatedMicroservice<TWebApiClaims> : WebApiMicroservice
        where TWebApiClaims : WebApiClaims
    {
#pragma warning disable CA1000
        /// <summary>
        /// Microservice instance (singleton)
        /// </summary>
        public static new WebApiJwtAuthenticatedMicroservice<TWebApiClaims>? Instance =>
            WebApiMicroservice.Instance as WebApiJwtAuthenticatedMicroservice<TWebApiClaims>;
#pragma warning restore CA1000

        /// <summary>
        /// Construct microservice instance
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public WebApiJwtAuthenticatedMicroservice(string[] args)
            : base(args)
        {
            WebApiUseAuthorizationByDefault = true;
        }

        protected override void OnCreateBuilder()
        {
            base.OnCreateBuilder();

            BuildJwtAuthenticator();
        }

        protected override void OnCreateApplication()
        {
            base.OnCreateApplication();

            CreateJwtAuthorization();
        }

#pragma warning disable IDE0058 // Expression value is never used
        private void BuildJwtAuthenticator()
        {
            var tokenSecret = new Jwt.TokenSecret(ConfigurationManager);

            Builder.Services.AddSingleton(tokenSecret);

            Builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.SaveToken = true;
#pragma warning disable CA5404 // Do not disable token validation checks
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(tokenSecret.KeyBytes),
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateTokenReplay = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero
                    };
#pragma warning restore CA5404 // Do not disable token validation checks
                });
        }
#pragma warning restore IDE0058 // Expression value is never used


        private static void CreateJwtAuthorization()
        {
            // TODO
        }
    }
}
