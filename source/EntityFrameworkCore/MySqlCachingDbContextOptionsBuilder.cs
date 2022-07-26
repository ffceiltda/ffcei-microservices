using FFCEI.Microservices.Configuration;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// CachingDbContextOptionsBuilder options builder with MySql support
    /// </summary>
    /// <typeparam name="TDbModelRepositoryContext">ModelRepositoryDbContext derived class</typeparam>
    public sealed class MySqlCachingDbContextOptionsBuilder<TDbModelRepositoryContext> : CachingDbContextOptionsBuilder<TDbModelRepositoryContext> where TDbModelRepositoryContext : DbContext
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="database">Initial database</param>
        /// <param name="userName">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="host">Host or IP address</param>
        /// <param name="port">TCP port</param>
        public MySqlCachingDbContextOptionsBuilder(string database, string userName, string password, string host, ushort port)
            : base(MySqlConnectionStringBuilder.Build(database, userName, password, host, port))
        {
        }

        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="configuration">MySqlConnectionConfiguration instance</param>
        public MySqlCachingDbContextOptionsBuilder(MySqlConnectionConfiguration configuration)
            : base(MySqlConnectionStringBuilder.Build(configuration))
        {
        }

        public override void ApplyOptions(DbContextOptionsBuilder options, IServiceProvider? serviceProvider = null)
        {
#pragma warning disable IDE0058 // Expression value is never used
            base.ApplyOptions(options, serviceProvider);

            options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString),
                parameters =>
                {
                    parameters.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.TinyInt1));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTime(MySqlDateTimeType.DateTime));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTimeOffset(MySqlDateTimeType.DateTime6));
                    parameters.DefaultDataTypeMappings(m => m.WithClrTimeSpan(MySqlTimeSpanType.Time6));
                    parameters.DefaultDataTypeMappings(m => m.WithClrTimeOnly(6));
                    parameters.EnableIndexOptimizedBooleanColumns();
                    parameters.EnableStringComparisonTranslations();
                    parameters.EnableRetryOnFailure().CommandTimeout(60);
                    parameters.UseMicrosoftJson();
                });
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
