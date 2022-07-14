using System.Globalization;

namespace FFCEI.Microservices.Configuration
{
    public static class MySqlConfigurationExtensionMethods
    {
        public static MySqlConfiguration GetMySqlConfiguration(this ConfigurationManager configurationManager,
            string mySqlHostSettingName = "MySql.Host",
            string mySqlPortSettingName = "MySql.Port",
            string mySqlUsernameSettingName = "MySql.Username",
            string mySqlPasswordSettingName = "MySql.Password",
            string mySqlDatabaseSettingName = "MySql.Database",
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

            var mySqlPort = configurationManager[mySqlPortSettingName];

            var result = new MySqlConfiguration
            {
                MySqlHost = configurationManager[mySqlHostSettingName],
                MySqlPort = (mySqlPort == null) ? null : ushort.Parse(mySqlPort, NumberStyles.Integer, CultureInfo.InvariantCulture),
                MySqlUsername = configurationManager[mySqlUsernameSettingName],
                MySqlPassword = configurationManager[mySqlPasswordSettingName],
                MySqlDatabase = configurationManager[mySqlDatabaseSettingName]
            };

            if (string.IsNullOrEmpty(result.MySqlHost) || (host is not null))
            {
                result.MySqlHost = host;
            }

            if ((result.MySqlPort is null) || (port is not null))
            {
                result.MySqlPort = port;
            }

            if (string.IsNullOrEmpty(result.MySqlUsername) || (username is not null))
            {
                result.MySqlUsername = username;
            }

            if (string.IsNullOrEmpty(result.MySqlPassword) || (password is not null))
            {
                result.MySqlPassword = password;
            }

            if (string.IsNullOrEmpty(result.MySqlDatabase) || (database is not null))
            {
                result.MySqlDatabase = database;
            }

            return result;
        }
    }
}
