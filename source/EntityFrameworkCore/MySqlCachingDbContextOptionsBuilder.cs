using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public sealed class MySqlCachingDbContextOptionsBuilder<TDbModelRepositoryContext> : CachingDbContextOptionsBuilder<TDbModelRepositoryContext> where TDbModelRepositoryContext : DbContext
    {
        public MySqlCachingDbContextOptionsBuilder(string database, string username, string password, string server, ushort port)
            : base(MySqlConnectionStringBuilder.Build(database, username, password, server, port))
        {
        }

        public override void ApplyOptions(DbContextOptionsBuilder options, IServiceProvider? serviceProvider = null)
        {
#pragma warning disable IDE0058 // Expression value is never used
            base.ApplyOptions(options, serviceProvider);

            options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString),
                parameters =>
                {
                    parameters.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTimeOffset(MySqlDateTimeType.DateTime));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTimeOffset(MySqlDateTimeType.DateTime6));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTimeOffset(MySqlDateTimeType.Timestamp));
                    parameters.DefaultDataTypeMappings(m => m.WithClrDateTimeOffset(MySqlDateTimeType.Timestamp6));
                    parameters.EnableStringComparisonTranslations();
                    parameters.EnableRetryOnFailure().CommandTimeout(60);
                    parameters.UseMicrosoftJson();
                });
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
