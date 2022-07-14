using FFCEI.Microservices.Configuration;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class MySqlConnectionStringBuilder
    {
        public static string Build(MySqlConfiguration configuration)
            => $"{configuration?.ConnectionString ?? string.Empty}OldGuids=true;Pooling=true;ConnectionLifeTime=15;ConnectionIdlePingTime=5;MaximumPoolsize=512;DefaultCommandTimeout=120";

        public static string Build(string database, string username, string password, string server, ushort port)
            => Build(new MySqlConfiguration() { MySqlDatabase = database, MySqlHost = server, MySqlPort = port, MySqlUsername = username, MySqlPassword = password });

    }
}
