using System.Globalization;

namespace FFCEI.Microservices.Configuration;

/// <summary>
/// Redis Connection Configuration extension methods
/// </summary>
public static class RedisConnectionConfigurationExtensionMethods
{
    /// <summary>
    /// Get redis connection configuration from Configuration Manager
    /// </summary>
    /// <param name="configurationManager">Configuration Manager instance</param>
    /// <param name="hostSettingName">Host setting name in Configuration Manager</param>
    /// <param name="portSettingName">Port setting name in Configuration Manager</param>
    /// <param name="usernameSettingName">UserName setting name in Configuration Manager</param>
    /// <param name="passwordSettingName">Password setting name in Configuration Manager</param>
    /// <param name="databaseSettingName">Database setting name in Configuration Manager</param>
    /// <param name="host">Use this Host or IP address if specified</param>
    /// <param name="port">Use this TCP port address if specified</param>
    /// <param name="userName">Use this UserName if specified</param>
    /// <param name="password">Use this Password if specified</param>
    /// <param name="database">Use this Database if specified</param>
    /// <returns>redisConnectionConfiguration instance</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static RedisConnectionConfiguration GetRedisConfiguration(this IConfigurationManager configurationManager,
        string hostSettingName = "Redis.Host",
        string portSettingName = "Redis.Port",
        string usernameSettingName = "Redis.UserName",
        string passwordSettingName = "Redis.Password",
        string databaseSettingName = "Redis.Database",
        string? host = null,
        ushort? port = null,
        string? userName = null,
        string? password = null,
        int? database = null)
    {
        ArgumentNullException.ThrowIfNull(configurationManager, nameof(configurationManager));

        var redisPort = configurationManager[portSettingName];
        var redisDatabase = configurationManager[databaseSettingName];

        var result = new RedisConnectionConfiguration
        {
            Host = configurationManager[hostSettingName],
            Port = (redisPort is null) ? null : ushort.Parse(redisPort, NumberStyles.Integer, CultureInfo.InvariantCulture),
            UserName = configurationManager[usernameSettingName],
            Password = configurationManager[passwordSettingName],
            Database = (redisDatabase is null) ? 0 : ushort.Parse(redisDatabase, NumberStyles.Integer, CultureInfo.InvariantCulture),
        };

        if (string.IsNullOrEmpty(result.Host) || (host is not null))
        {
            result.Host = host;
        }

        if ((result.Port is null) || (port is not null))
        {
            result.Port = port;
        }

        if (string.IsNullOrEmpty(result.UserName) || (userName is not null))
        {
            result.UserName = userName;
        }

        if (string.IsNullOrEmpty(result.Password) || (password is not null))
        {
            result.Password = password;
        }

        if (database is not null)
        {
            result.Database = database.Value;
        }

        return result;
    }
}
