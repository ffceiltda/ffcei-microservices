using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FFCEI.Microservices.AspNetCore
{
    public class WebApiJwtAuthenticatedMicroservice<TWebApiClaims> : WebApiMicroservice
        where TWebApiClaims : WebApiClaims
    {
#pragma warning disable CA1000
        public new static WebApiJwtAuthenticatedMicroservice<TWebApiClaims>? Instance =>
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

        private void BuildJwtAuthenticator(WebApplicationBuilder builder)
        {
            var tokenSecret = new Jwt.TokenSecret(ConfigurationManager);

            builder.Services.AddSingleton(tokenSecret);
        }
    }
}
