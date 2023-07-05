namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// Configuration Manager interface (with support for system environment, environment files and ASP.NET Core appSettings)
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Reload configuration from configuration sources
        /// </summary>
        void ReloadConfiguration();

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>Setting value or null if not found</returns>
        string? this[string key] { get; }

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>Setting value or null if not found</returns>
        string? GetKey(string key);

        /// <summary>
        /// Check if a configuration exists on repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>true if found, false otherwise</returns>
        bool HasKey(string key);

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <param name="value">Value found, or null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        bool TryGetKey(string key, out string? value);
    }
}
