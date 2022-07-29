using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace FFCEI.Microservices.Configuration;

/// <summary>
/// Configuration Manager (with support for system environment, environment files and ASP.NET Core appSettings)
/// </summary>
public sealed class ConfigurationManager : IConfigurationManager
{
    private Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private bool _isDevelopment = Debugger.IsAttached;
    private bool _isProduction = !Debugger.IsAttached;
    private string _allConfigurationsFilePath = string.Empty;
    private IEnumerable<string>? _allConfigurations;
    private string _applicationConfigurationsFilePath = string.Empty;
    private IEnumerable<string>? _applicationConfigurations;

    internal ConfigurationManager(WebApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        _configuration = builder.Configuration;

        _isDevelopment = builder.Environment.IsDevelopment();
        _isProduction = builder.Environment.IsProduction();

        LoadConfiguration();
    }

    internal ConfigurationManager(IHostBuilder builder, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        _configuration = configuration;

        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        var mainAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        var mainAssemblyCodeBase = Assembly.GetEntryAssembly()?.Location;

        if (!string.IsNullOrEmpty(mainAssemblyCodeBase))
        {
            var mainAssemblyFilenamePath = Path.GetDirectoryName(mainAssemblyCodeBase);

            while (!string.IsNullOrEmpty(mainAssemblyFilenamePath) &&
                (string.IsNullOrEmpty(_allConfigurationsFilePath) || string.IsNullOrEmpty(_applicationConfigurationsFilePath)))
            {
                var environmentBasePath = Path.Combine(mainAssemblyFilenamePath, "Environment");

                if (Directory.Exists(environmentBasePath))
                {
                    if (_isDevelopment)
                    {
                        environmentBasePath = Path.Combine(environmentBasePath, "Development");
                    }
                    else if (_isProduction)
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

                                file.Close();

                                _allConfigurations = File.ReadAllLines(allConfigurationsFilePath);
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

                                file.Close();

                                _allConfigurations = File.ReadAllLines(allConfigurationsFilePath);
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

                                file.Close();

                                _applicationConfigurations = File.ReadAllLines(applicationConfigurationsFilePath);
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

                                file.Close();

                                _applicationConfigurations = File.ReadAllLines(applicationConfigurationsFilePath);
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

    public string? this[string key]
    {
        get
        {
            return GetKey(key);
        }
    }

    public string? GetKey(string key)
    {
        return TryGetKey(key, out var value) ? value : null;
    }

    public bool HasKey(string key)
    {
        return TryGetKey(key, out var _);
    }

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

    private static bool TryGetKeyFromConfigurationFile(IEnumerable<string>? lines, string key, out string? value)
    {
        if (lines?.FirstOrDefault() is null)
        {
            value = null;

            return false;
        }

        var environmentKey = FormatKeyAsEnvironmentVariable(key);

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
        return TryGetKeyFromConfigurationFile(_applicationConfigurations, key, out value);
    }

    private bool TryGetKeyFromAllConfigurations(string key, out string? value)
    {
        return TryGetKeyFromConfigurationFile(_allConfigurations, key, out value);
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
