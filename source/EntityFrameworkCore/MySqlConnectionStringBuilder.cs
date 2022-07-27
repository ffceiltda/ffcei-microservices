using FFCEI.Microservices.Configuration;

namespace FFCEI.Microservices.EntityFrameworkCore;

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
        => $"{configuration?.ConnectionString ?? string.Empty}";

    /// <summary>
    /// Build a MySql ConnectionString
    /// </summary>
    /// <param name="database">Initial database</param>
    /// <param name="userName">UserName</param>
    /// <param name="password">Password</param>
    /// <param name="host">Host or IP address</param>
    /// <param name="port">TCP port</param>
    /// <returns>Connection string</returns>
    public static string Build(string database, string userName, string password, string host, ushort port)
        => Build(new MySqlConnectionConfiguration() { Database = database, Host = host, Port = port, UserName = userName, Password = password });

}
