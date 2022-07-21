using FFCEI.Microservices.Configuration;
using FFCEI.Microservices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// IServiceCollection extension methods
    /// </summary>
    public static class ServiceCollectionExtensionMethods
    {
        /// <summary>
        /// Add DbContext for MySql
        /// </summary>
        /// <typeparam name="TDbContext">ModelRepositoryDbContext derived DbContext class</typeparam>
        /// <param name="serviceCollection">IServiceCollection instance</param>
        /// <param name="mySqlConnectionConfiguration">MySqlConnectionConfiguration instance</param>
        /// <returns>IServiceCollection instance</returns>
        public static IServiceCollection AddMySqlDbContext<TDbContext>(this IServiceCollection serviceCollection, MySqlConnectionConfiguration mySqlConnectionConfiguration)
            where TDbContext : ModelRepositoryDbContext
        => serviceCollection.AddDbContext<TDbContext>((serviceProvider, options) =>
            { new MySqlCachingDbContextOptionsBuilder<TDbContext>(mySqlConnectionConfiguration).ApplyOptions(options, serviceProvider); });

        /// <summary>
        /// Add ModelRepositoryContainer
        /// </summary>
        /// <typeparam name="TDbContext">ModelRepositoryDbContext derived DbContext class</typeparam>
        /// <typeparam name="TIModelRepositoryContainer">IModelRepositoryContainer derived interface</typeparam>
        /// <typeparam name="TModelRepositoryContainer">ModelRepositoryContainer derived class</typeparam>
        /// <param name="serviceCollection">IServiceCollection instance</param>
        /// <returns>IServiceCollection instance</returns>
        public static IServiceCollection AddModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>(
            this IServiceCollection serviceCollection)
            where TDbContext : ModelRepositoryDbContext
            where TIModelRepositoryContainer : class, IModelRepositoryContainer
            where TModelRepositoryContainer : ModelRepositoryContainer<TDbContext>, TIModelRepositoryContainer
        => serviceCollection.AddTransient<TIModelRepositoryContainer, TModelRepositoryContainer>();

        /// <summary>
        /// Add DbContext for MySql and add ModelRepositoryContainer associated
        /// </summary>
        /// <typeparam name="TDbContext">ModelRepositoryDbContext derived DbContext class</typeparam>
        /// <typeparam name="TIModelRepositoryContainer">IModelRepositoryContainer derived interface</typeparam>
        /// <typeparam name="TModelRepositoryContainer">ModelRepositoryContainer derived class</typeparam>
        /// <param name="serviceCollection">IServiceCollection instance</param>
        /// <param name="mySqlConnectionConfiguration">MySqlConnectionConfiguration instance</param>
        /// <returns>IServiceCollection instance</returns>
        public static IServiceCollection AddMySqlDbContextAndModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>(
            this IServiceCollection serviceCollection, MySqlConnectionConfiguration mySqlConnectionConfiguration)
            where TDbContext : ModelRepositoryDbContext
            where TIModelRepositoryContainer : class, IModelRepositoryContainer
            where TModelRepositoryContainer : ModelRepositoryContainer<TDbContext>, TIModelRepositoryContainer
        => serviceCollection.AddMySqlDbContext<TDbContext>(mySqlConnectionConfiguration).
            AddModelRepositoryContainer<TDbContext, TIModelRepositoryContainer, TModelRepositoryContainer>();

        /// <summary>
        /// Add Api service
        /// </summary>
        /// <typeparam name="TIApiService">Api service interface</typeparam>
        /// <typeparam name="TApiService">Api service implemmentation</typeparam>
        /// <param name="serviceCollection">IServiceCollection instance</param>
        /// <returns>IServiceCollection instance</returns>
        public static IServiceCollection AddApiService<TIApiService, TApiService>(this IServiceCollection serviceCollection)
            where TIApiService : class, IApiService
            where TApiService : ApiService, TIApiService
        => serviceCollection.AddTransient<TIApiService, TApiService>();
    }
}
