using System.Text;

namespace FFCEI.Microservices.Configuration
{
    public sealed class MySqlConfiguration
    {
        public string? MySqlHost { get; set; }
        public ushort? MySqlPort { get; set; }
        public string? MySqlUsername { get; set; }
        public string? MySqlPassword { get; set; }
        public string? MySqlDatabase { get; set; }

        public string ConnectionString
        {
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
            get
            {
                var stringBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(MySqlHost))
                {
                    stringBuilder.Append($"Server={MySqlHost};");
                }

                if ((MySqlPort != null) && (MySqlPort.Value != 0))
                {
                    stringBuilder.Append($"Port={MySqlPort};");
                }

                if (!string.IsNullOrEmpty(MySqlUsername))
                {
                    stringBuilder.Append($"User={MySqlUsername};");
                }

                if (!string.IsNullOrEmpty(MySqlPassword))
                {
                    stringBuilder.Append($"Password={MySqlPassword};");
                }

                if (!string.IsNullOrEmpty(MySqlDatabase))
                {
                    stringBuilder.Append($"Database={MySqlDatabase};");
                }

                return stringBuilder.ToString();
            }
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
