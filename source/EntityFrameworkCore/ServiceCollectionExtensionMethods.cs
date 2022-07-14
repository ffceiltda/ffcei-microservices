using FFCEI.Microservices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection AddMySqlDbContext<TDbContext>(this IServiceCollection serviceCollection, MySqlConfiguration configuration)
            where TDbContext : ModelRepositoryDbContext
        => serviceCollection.AddDbContext<TDbContext>((serviceProvider, options) =>
            { new MySqlCachingDbContextOptionsBuilder<TDbContext>(configuration).ApplyOptions(options, serviceProvider); });

        public static IServiceCollection AddModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>(
            this IServiceCollection serviceCollection)
            where TDbContext : ModelRepositoryDbContext
            where TIModelRepositoryContainer : class, IModelRepositoryContainer
            where TModelRepositoryContainer : ModelRepositoryContainer<TDbContext>, TIModelRepositoryContainer
        => serviceCollection.AddScoped<TIModelRepositoryContainer, TModelRepositoryContainer>();

        public static IServiceCollection AddMySqlDbContextAndModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>(
            this IServiceCollection serviceCollection, MySqlConfiguration configuration)
            where TDbContext : ModelRepositoryDbContext
            where TIModelRepositoryContainer : class, IModelRepositoryContainer
            where TModelRepositoryContainer : ModelRepositoryContainer<TDbContext>, TIModelRepositoryContainer
        => serviceCollection.AddMySqlDbContext<TDbContext>(configuration).
            AddModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>();
    }
}
