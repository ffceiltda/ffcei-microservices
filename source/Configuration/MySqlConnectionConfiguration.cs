using System.Text;

namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// MySql connection configuration
    /// </summary>
    public sealed class MySqlConnectionConfiguration : ConnectionConfiguration
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
        public string? UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Initial database
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// Handle binary(16) as Uuid
        /// </summary>
        public bool OldGuids { get; set; }

        /// <summary>
        /// Use compressed protocol
        /// </summary>
        public bool UseCompression { get; set; }

        /// <summary>
        /// Connection pooling
        /// </summary>
        public bool Pooling { get; set; } = true;

        /// <summary>
        /// Minimum Pool Size
        /// </summary>
        public ushort MinimumPoolSize { get; set; } = 8;

        /// <summary>
        /// Maximum Pool Size
        /// </summary>
        public ushort MaximumPoolSize { get; set; } = 128;

        /// <summary>
        /// Connection lifetime
        /// </summary>
        public ushort ConnectionLifeTime { get; set; } = 30;

        /// <summary>
        /// Connection reset when get from pool
        /// </summary>
        public bool ConnectionReset { get; set; } = true;

        /// <summary>
        /// Connection idle ping time
        /// </summary>
        public ushort ConnectionIdlePingTime { get; set; } = 10;

        /// <summary>
        /// Connection timeout
        /// </summary>
        public ushort ConnectionTimeout { get; set; } = 10;

        /// <summary>
        /// Default command timeout
        /// </summary>
        public ushort DefaultCommandTimeout { get; set; } = 120;

        /// <summary>
        /// Treat TINYINT(1) as System.Boolean
        /// </summary>
        public bool TreatTinyAsBoolean { get; set; }

        /// <summary>
        /// DateTime UTC/Local handling type
        /// </summary>
        public DateTimeKind DateTimeKind { get; set; } = DateTimeKind.Utc;

        protected override StringBuilder BuildConnectionString()
        {
            var stringBuilder = new StringBuilder();

#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
            if (!string.IsNullOrEmpty(Host))
            {
                stringBuilder.Append($"Server={Host};");
            }

            if ((Port is not null) && (Port.Value != 0))
            {
                stringBuilder.Append($"Port={Port};");
            }

            if (!string.IsNullOrEmpty(UserName))
            {
                stringBuilder.Append($"User={UserName};");
            }

            if (!string.IsNullOrEmpty(Password))
            {
                stringBuilder.Append($"Password={Password};");
            }

            if (!string.IsNullOrEmpty(Database))
            {
                stringBuilder.Append($"Database={Database};");
            }

            stringBuilder.Append($"OldGuids={OldGuids};");
            stringBuilder.Append($"UseCompression={UseCompression};");
            stringBuilder.Append($"Pooling={Pooling};");

            if (Pooling)
            {
                stringBuilder.Append($"MinimumPoolSize={MinimumPoolSize};");
                stringBuilder.Append($"MaximumPoolSize={MaximumPoolSize};");
                stringBuilder.Append($"ConnectionLifeTime={ConnectionLifeTime};");
                stringBuilder.Append($"ConnectionReset={ConnectionReset};");
                stringBuilder.Append($"ConnectionIdlePingTime={ConnectionIdlePingTime};");
            }

            stringBuilder.Append($"ConnectionTimeout={ConnectionTimeout};");
            stringBuilder.Append($"DefaultCommandTimeout={DefaultCommandTimeout};");
            stringBuilder.Append($"TreatTinyAsBoolean={TreatTinyAsBoolean};");
            stringBuilder.Append($"DateTimeKind={DateTimeKind};");
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore IDE0058 // Expression value is never used

            return stringBuilder;
        }
    }
}
