using System.Text;

namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// MySql connection configuration
    /// </summary>
    public sealed class MySqlConnectionConfiguration
    {
        /// <summary>
        /// Hostname or IP address
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// TCP port
        /// </summary>
        public ushort? Port { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Initial database
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// Basic connection string
        /// </summary>
        public string ConnectionString
        {
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
            get
            {
                var stringBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(Host))
                {
                    stringBuilder.Append($"Server={Host};");
                }

                if ((Port != null) && (Port.Value != 0))
                {
                    stringBuilder.Append($"Port={Port};");
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    stringBuilder.Append($"User={Username};");
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    stringBuilder.Append($"Password={Password};");
                }

                if (!string.IsNullOrEmpty(Database))
                {
                    stringBuilder.Append($"Database={Database};");
                }

                return stringBuilder.ToString();
            }
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
