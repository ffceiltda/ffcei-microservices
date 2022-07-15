using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// DbContext options builder if SecondLevelCache interception support
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class CachingDbContextOptionsBuilder<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public CachingDbContextOptionsBuilder(string connectionString)
        {
            ConnectionString = connectionString;
        }

#pragma warning disable IDE0058 // Expression value is never used
        /// <summary>
        /// Apply options to serviceProvider
        /// </summary>
        /// <param name="options">DbContext options builder instance</param>
        /// <param name="serviceProvider">ServiceProvider instance</param>
        /// <exception cref="ArgumentNullException">Throw if options is null</exception>
        public virtual void ApplyOptions(DbContextOptionsBuilder options, IServiceProvider? serviceProvider = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.UseLazyLoadingProxies();

            if (Debugger.IsAttached)
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }

            var interceptor = serviceProvider?.GetService<SecondLevelCacheInterceptor>();

            if (interceptor is not null)
            {
                options.AddInterceptors(interceptor);
            }
        }
#pragma warning restore IDE0058 // Expression value is never used
    }
}
