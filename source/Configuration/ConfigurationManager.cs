using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Reflection;

namespace FFCEI.Microservices.Configuration
{
    /// <summary>
    /// Configuration Manager (with support for system environment, environment files and ASP.NET Core appSettings)
    /// </summary>
    public sealed class ConfigurationManager
    {
        private Microsoft.Extensions.Configuration.ConfigurationManager _configuration;
        private string _allConfigurationsFilePath = string.Empty;
        private string _applicationConfigurationsFilePath = string.Empty;

        internal ConfigurationManager(WebApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _configuration = builder.Configuration;

            var mainAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            var mainAssemblyCodeBase = Assembly.GetEntryAssembly()?.Location;

            if (!string.IsNullOrEmpty(mainAssemblyCodeBase))
            {
                string environmentBasePath;

                var mainAssemblyCodeBaseUri = new UriBuilder(mainAssemblyCodeBase);
                var mainAssemblyCodeBasePath = mainAssemblyCodeBaseUri.Path;
                var mainAssemblyFilenamePath = Path.GetDirectoryName(mainAssemblyCodeBasePath);

                while (!string.IsNullOrEmpty(mainAssemblyFilenamePath) &&
                    (string.IsNullOrEmpty(_allConfigurationsFilePath) || string.IsNullOrEmpty(_applicationConfigurationsFilePath)))
                {
                    environmentBasePath = Path.Combine(mainAssemblyFilenamePath, "Environment");

                    if (Directory.Exists(environmentBasePath))
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            environmentBasePath = Path.Combine(environmentBasePath, "Development");
                        }

                        if (builder.Environment.IsProduction())
                        {
                            environmentBasePath = Path.Combine(environmentBasePath, "Production");
                        }

                        var environmentUserBasePath = Path.Combine(environmentBasePath, Environment.UserName);

#pragma warning disable CA1031 // Do not catch general exception types
                        if (string.IsNullOrEmpty(_allConfigurationsFilePath))
                        {
                            var allConfigurationsFilePath = Path.Combine(environmentUserBasePath, "ALL.env");

                            if (File.Exists(allConfigurationsFilePath))
                            {
                                try
                                {
                                    using var file = File.Open(allConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                                    _allConfigurationsFilePath = allConfigurationsFilePath;
                                }
                                catch
                                {
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(_allConfigurationsFilePath))
                        {
                            var allConfigurationsFilePath = Path.Combine(environmentBasePath, "ALL.env");

                            if (File.Exists(allConfigurationsFilePath))
                            {
                                try
                                {
                                    using var file = File.Open(allConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                                    _allConfigurationsFilePath = allConfigurationsFilePath;
                                }
                                catch
                                {
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(_applicationConfigurationsFilePath))
                        {
                            var applicationConfigurationsFilePath = Path.Combine(environmentUserBasePath, $"{mainAssemblyName}.env");

                            if (File.Exists(applicationConfigurationsFilePath))
                            {
                                try
                                {
                                    using var file = File.Open(applicationConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                                    _applicationConfigurationsFilePath = applicationConfigurationsFilePath;
                                }
                                catch
                                {
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(_applicationConfigurationsFilePath))
                        {
                            var applicationConfigurationsFilePath = Path.Combine(environmentBasePath, $"{mainAssemblyName}.env");

                            if (File.Exists(applicationConfigurationsFilePath))
                            {
                                try
                                {
                                    using var file = File.Open(applicationConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                                    _applicationConfigurationsFilePath = applicationConfigurationsFilePath;
                                }
                                catch
                                {
                                }
                            }
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }

                    var parentDirectory = Path.GetDirectoryName(mainAssemblyFilenamePath);

                    if (parentDirectory == mainAssemblyFilenamePath)
                    {
                        mainAssemblyFilenamePath = null;
                    }
                    else
                    {
                        mainAssemblyFilenamePath = parentDirectory;
                    }
                }
            }
        }

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>Setting value or null if not found</returns>
        public string? this[string key]
        {
            get
            {
                return GetKey(key);
            }
        }

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>Setting value or null if not found</returns>
        public string? GetKey(string key)
        {
            return TryGetKey(key, out var value) ? value : null;
        }

        /// <summary>
        /// Check if a configuration exists on repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <returns>true if found, false otherwise</returns>
        public bool HasKey(string key)
        {
            return TryGetKey(key, out var _);
        }

        /// <summary>
        /// Obtains a configuration from repositories
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <param name="value">Value found, or null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryGetKey(string key, out string? value)
        {
            if (TryGetKeyFromApplicationConfigurations(key, out value))
            {
                return true;
            }

            if (TryGetKeyFromAllConfigurations(key, out value))
            {
                return true;
            }

            if (TryGetKeyFromEnvironmentVariable(key, out value))
            {
                return true;
            }

            if (TryGetKeyFromConfigurationManager(key, out value))
            {
                return true;
            }

            return false;
        }

        private static string FormatKeyAsEnvironmentVariable(string key)
        {
            return key.Replace(".", "_", StringComparison.InvariantCulture).
                Replace(":", "_", StringComparison.InvariantCulture).ToUpper(CultureInfo.InvariantCulture);
        }

        private static bool TryGetKeyFromConfigurationFile(string filename, string key, out string? value)
        {
            if (string.IsNullOrEmpty(filename))
            {
                value = null;

                return false;
            }

            var environmentKey = FormatKeyAsEnvironmentVariable(key);

            var lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
                var equalIndex = line.IndexOf('=', StringComparison.InvariantCulture);

                if (equalIndex == -1)
                {
                    continue;
                }

                var lineKey = line.Substring(0, equalIndex).Replace("export", "", StringComparison.InvariantCulture).
                    TrimStart().TrimEnd().ToUpper(CultureInfo.InvariantCulture);

                if (lineKey == environmentKey)
                {
                    value = line.Substring(equalIndex + 1).TrimStart().TrimEnd();

                    return true;
                }
            }

            value = null;

            return false;
        }

        private bool TryGetKeyFromApplicationConfigurations(string key, out string? value)
        {
            return TryGetKeyFromConfigurationFile(_applicationConfigurationsFilePath, key, out value);
        }

        private bool TryGetKeyFromAllConfigurations(string key, out string? value)
        {
            return TryGetKeyFromConfigurationFile(_allConfigurationsFilePath, key, out value);
        }

        private static bool TryGetKeyFromEnvironmentVariable(string key, out string? value)
        {
            var environmentKey = FormatKeyAsEnvironmentVariable(key);

            value = Environment.GetEnvironmentVariable(environmentKey);

            return !string.IsNullOrEmpty(value);
        }

        private bool TryGetKeyFromConfigurationManager(string key, out string? value)
        {
            value = _configuration[key];

            if (string.IsNullOrEmpty(value))
            {
                var environmentKey = FormatKeyAsEnvironmentVariable(key);

                value = _configuration[environmentKey];
            }

            return !string.IsNullOrEmpty(value);
        }
    }
}
