using FFCEI.Microservices.AspNetCore;
using FFCEI.Microservices.Microservices;

Microservice.RegistryPathForEnvironment = "SOFTWARE\\FFCEI";

// Create Web API microservice template
var microservice = new WebApiJwtAuthenticatedMicroservice<WebApiClaims>(args);

microservice.WebApiGenerateSwagger = true;

// TODO: use builder to configure dependency injection as standard ASP.NET Core
// var builder = microservice.Builder;

// TODO: use builder to setup application as standard ASP.NET Core
// var application = microservice.Application;

// Run your microservice
microservice.Run();
