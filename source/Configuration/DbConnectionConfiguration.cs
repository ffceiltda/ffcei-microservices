using System.Text;

namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// DbConnection configuration
    /// </summary>
    public abstract class DbConnectionConfiguration
    {
        /// <summary>
        /// Basic connection string
        /// </summary>
        public string ConnectionString
        {
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
            get
            {
                return BuildConnectionString().ToString();
            }
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore IDE0058 // Expression value is never used
        }

        protected abstract StringBuilder BuildConnectionString();
    }
}
