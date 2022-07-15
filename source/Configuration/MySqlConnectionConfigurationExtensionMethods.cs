using System.Globalization;

namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class MySqlConnectionConfigurationExtensionMethods
    {
        /// <summary>
        /// Get MySQL connection configuration from Configuration Manager
        /// </summary>
        /// <param name="configurationManager">Configuration Manager instance</param>
        /// <param name="hostSettingName">Host setting name in Configuration Manager</param>
        /// <param name="portSettingName">Port setting name in Configuration Manager</param>
        /// <param name="usernameSettingName">Username setting name in Configuration Manager</param>
        /// <param name="passwordSettingName">Password setting name in Configuration Manager</param>
        /// <param name="databaseSettingName">Database setting name in Configuration Manager</param>
        /// <param name="host">Use this Host or IP address if specified</param>
        /// <param name="port">Use this TCP port address if specified</param>
        /// <param name="username">Use this Username if specified</param>
        /// <param name="password">Use this Password if specified</param>
        /// <param name="database">Use this Database if specified</param>
        /// <returns>MySqlConnectionConfiguration instance</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MySqlConnectionConfiguration GetMySqlConfiguration(this ConfigurationManager configurationManager,
            string hostSettingName = "MySql.Host",
            string portSettingName = "MySql.Port",
            string usernameSettingName = "MySql.Username",
            string passwordSettingName = "MySql.Password",
            string databaseSettingName = "MySql.Database",
            string? host = null,
            ushort? port = null,
            string? username = null,
            string? password = null,
            string? database = null)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            var mySqlPort = configurationManager[portSettingName];

            var result = new MySqlConnectionConfiguration
            {
                Host = configurationManager[hostSettingName],
                Port = (mySqlPort == null) ? null : ushort.Parse(mySqlPort, NumberStyles.Integer, CultureInfo.InvariantCulture),
                Username = configurationManager[usernameSettingName],
                Password = configurationManager[passwordSettingName],
                Database = configurationManager[databaseSettingName]
            };

            if (string.IsNullOrEmpty(result.Host) || (host is not null))
            {
                result.Host = host;
            }

            if ((result.Port is null) || (port is not null))
            {
                result.Port = port;
            }

            if (string.IsNullOrEmpty(result.Username) || (username is not null))
            {
                result.Username = username;
            }

            if (string.IsNullOrEmpty(result.Password) || (password is not null))
            {
                result.Password = password;
            }

            if (string.IsNullOrEmpty(result.Database) || (database is not null))
            {
                result.Database = database;
            }

            return result;
        }
    }
}
