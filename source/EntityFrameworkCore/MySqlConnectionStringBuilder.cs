using FFCEI.Microservices.Configuration;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    /// <summary>
    /// Build a MySql ConnectionString
    /// </summary>
    public static class MySqlConnectionStringBuilder
    {
        /// <summary>
        /// Build a MySql ConnectionString
        /// </summary>
        /// <param name="configuration">MySqlConnectionConfiguration instance</param>
        /// <returns>Connection string</returns>
        public static string Build(MySqlConnectionConfiguration configuration)
            => $"{configuration?.ConnectionString ?? string.Empty}OldGuids=true;Pooling=true;ConnectionLifeTime=15;ConnectionIdlePingTime=5;MaximumPoolsize=512;DefaultCommandTimeout=120";

        /// <summary>
        /// Build a MySql ConnectionString
        /// </summary>
        /// <param name="database">Initial database</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="host">Host or IP address</param>
        /// <param name="port">TCP port</param>
        /// <returns>Connection string</returns>
        public static string Build(string database, string username, string password, string host, ushort port)
            => Build(new MySqlConnectionConfiguration() { Database = database, Host = host, Port = port, Username = username, Password = password });

    }
}
