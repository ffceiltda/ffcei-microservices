using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public class CachingDbContextOptionsBuilder<TDbContext> where TDbContext : DbContext
    {
        public CachingDbContextOptionsBuilder(string connectionString, DbContextOptionsBuilder options, IServiceProvider? serviceProvider = null)
            : base()
        {
        }

#pragma warning disable IDE0058 // Expression value is never used
        public virtual void ApplyOptions(string connectionString, DbContextOptionsBuilder options, IServiceProvider? serviceProvider = null)
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
