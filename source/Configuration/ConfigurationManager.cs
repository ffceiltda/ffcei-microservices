using FFCEI.Microservices.Microservices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FFCEI.Microservices.Configuration;

/// <summary>
/// Configuration Manager (with support for system environment, environment files and ASP.NET Core appSettings)
/// </summary>
public sealed class ConfigurationManager : IConfigurationManager
{
    private Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private Microsoft.Extensions.Logging.ILogger _logger;
    private bool _isDevelopment = Debugger.IsAttached;
    private bool _isProduction = !Debugger.IsAttached;
    private string _allConfigurationsFilePath = string.Empty;
    private IEnumerable<string>? _allConfigurations;
    private string _applicationConfigurationsFilePath = string.Empty;
    private IEnumerable<string>? _applicationConfigurations;

    internal ConfigurationManager(ILogger logger, WebApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        _logger = logger;
        _configuration = builder.Configuration;

        _isDevelopment = builder.Environment.IsDevelopment();
        _isProduction = builder.Environment.IsProduction();

        LoadConfiguration();
    }

    internal ConfigurationManager(ILogger logger, IHostBuilder builder, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        _logger = logger;
        _configuration = configuration;

        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        var mainAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

        if (string.IsNullOrEmpty(mainAssemblyName))
        {
            return;
        }

        var mainAssemblyCodeBase = Assembly.GetEntryAssembly()?.Location;
        var mainAssemblySearchPaths = new List<string>();

        if (!string.IsNullOrEmpty(mainAssemblyCodeBase))
        {
            var searchPath = Path.GetDirectoryName(mainAssemblyCodeBase);

            while (!string.IsNullOrEmpty(searchPath))
            {
                if (Directory.Exists(searchPath))
                {
                    mainAssemblySearchPaths.Add(searchPath);
                }

#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    var parent = Directory.GetParent(searchPath);

                    searchPath = parent?.FullName;
                }
                catch
                {
                    break;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        mainAssemblySearchPaths.Reverse();

        var configurationSearchPath = new List<string>();
        var configurationSearchUserName = new List<string?>();

        foreach (var mainAssemblySearchPath in mainAssemblySearchPaths)
        {
            if (InsertDirectoryInSearchPath(ref configurationSearchPath, mainAssemblySearchPath))
            {
                configurationSearchUserName.Insert(0, null);
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            (var machineSettingsPath, var machineSettingsUserName) = TryLoadEnvironmentSettingsFromRegistry(Registry.LocalMachine);

            if (InsertDirectoryInSearchPath(ref configurationSearchPath, machineSettingsPath))
            {
                configurationSearchUserName.Insert(0, machineSettingsUserName);
            }

            (var userSettingsPath, var userSettingsUserName) = TryLoadEnvironmentSettingsFromRegistry(Registry.CurrentUser);

            if (InsertDirectoryInSearchPath(ref configurationSearchPath, userSettingsPath))
            {
                configurationSearchUserName.Insert(0, userSettingsUserName);
            }
        }

        TryLoadEnvironmentSettingsFromPath(mainAssemblyName, configurationSearchPath, configurationSearchUserName);
    }

    private static bool InsertDirectoryInSearchPath(ref List<string> searchPaths, string? path)
    {
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path) && (searchPaths.IndexOf(path) == -1))
        {
            searchPaths.Insert(0, path);

            return true;
        }

        return false;
    }

    private static (string? registryPath, string? registryUserName) TryLoadEnvironmentSettingsFromRegistry(RegistryKey registryKey)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!string.IsNullOrEmpty(Microservice.RegistryPathForEnvironment))
            {
                try
                {
                    string? registryPath = null;
                    string? registryUserName = null;

                    using (var key = registryKey.OpenSubKey(Microservice.RegistryPathForEnvironment))
                    {
                        var kind = key?.GetValueKind("EnvironmentFilesPath") ?? RegistryValueKind.None;

                        if (kind == RegistryValueKind.String)
                        {
                            registryPath = key?.GetValue("EnvironmentFilesPath")?.ToString();
                        }

                        kind = key?.GetValueKind("EnvironmentUserName") ?? RegistryValueKind.None;

                        if (kind == RegistryValueKind.String)
                        {
                            registryUserName = key?.GetValue("EnvironmentUserName")?.ToString();
                        }

                        return (registryPath, registryUserName);
                    }
                }
                catch
                {
                }
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return (null, null);
    }

    private void TryLoadEnvironmentSettingsFromPath(string mainAssemblyName, List<string> environmentSearchPaths, List<string?> environmentUserNames)
    {
        var searchPaths = new List<string>();

        foreach (var environmentSearchPath in environmentSearchPaths)
        {
            if (!Directory.Exists(environmentSearchPath))
            {
                continue;
            }

            foreach (var environmentUserName in environmentUserNames)
            {
                var userNameToCombine = string.IsNullOrEmpty(environmentUserName) ? Environment.UserName : environmentUserName;

                if (InsertDirectoryInSearchPath(ref searchPaths, environmentSearchPath))
                {
                    var environmentSearchUserPath = Path.Combine(environmentSearchPath, userNameToCombine);

                    var _ = InsertDirectoryInSearchPath(ref searchPaths, environmentSearchUserPath);
                }

                var environmentPath = Path.Combine(environmentSearchPath, "Environment");

                if (InsertDirectoryInSearchPath(ref searchPaths, environmentPath))
                {
                    var environmentUserPath = Path.Combine(environmentPath, userNameToCombine);

                    var _ = InsertDirectoryInSearchPath(ref searchPaths, environmentUserPath);
                }

                var environmentRuntimePath = environmentPath;

                if (_isDevelopment)
                {
                    environmentRuntimePath = Path.Combine(environmentRuntimePath, "Development");
                }
                else if (_isProduction)
                {
                    environmentRuntimePath = Path.Combine(environmentRuntimePath, "Production");
                }

                if (InsertDirectoryInSearchPath(ref searchPaths, environmentRuntimePath))
                {
                    var environmentRuntimeUserPath = Path.Combine(environmentRuntimePath, userNameToCombine);

                    var _ = InsertDirectoryInSearchPath(ref searchPaths, environmentRuntimeUserPath);
                }
            }
        }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
        _logger.LogInformation($"Configuration search path: {searchPaths.Count}");

        foreach (var searchPath in searchPaths)
        {
            _logger.LogInformation(searchPath);
        }
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        foreach (var searchPath in searchPaths)
        {
            if (!string.IsNullOrEmpty(_allConfigurationsFilePath) &&
                !string.IsNullOrEmpty(_applicationConfigurationsFilePath))
            {
                break;
            }

            if (string.IsNullOrEmpty(_allConfigurationsFilePath))
            {
                var allConfigurationsFilePath = Path.Combine(searchPath, "ALL.env");

                if (File.Exists(allConfigurationsFilePath))
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                        _logger.LogInformation($"Trying to load system-wide configurations from {allConfigurationsFilePath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates

                        using var file = File.Open(allConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                        file.Close();

                        _allConfigurations = File.ReadAllLines(allConfigurationsFilePath);
                        _allConfigurationsFilePath = allConfigurationsFilePath;

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                        _logger.LogInformation($"Loaded system-wide configurations from {allConfigurationsFilePath}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    }
                    catch
                    {
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
                else
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogWarning($"File ALL.env not found in {searchPath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }

            if (string.IsNullOrEmpty(_applicationConfigurationsFilePath))
            {
                var applicationConfigurationsFilePath = Path.Combine(searchPath, $"{mainAssemblyName}.env");

                if (File.Exists(applicationConfigurationsFilePath))
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                        _logger.LogInformation($"Trying to load application-specific configurations from {applicationConfigurationsFilePath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates

                        using var file = File.Open(applicationConfigurationsFilePath, FileMode.Open, FileAccess.Read);

                        file.Close();

                        _applicationConfigurations = File.ReadAllLines(applicationConfigurationsFilePath);
                        _applicationConfigurationsFilePath = applicationConfigurationsFilePath;

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                        _logger.LogInformation($"Loaded application-specific configurations from {applicationConfigurationsFilePath}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    }
                    catch
                    {
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
                else
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogWarning($"File {mainAssemblyName}.env not found in {searchPath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }
        }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
        if (string.IsNullOrEmpty(_allConfigurationsFilePath))
        {
            _logger.LogWarning("cannot load file ALL.env");
        }

        if (string.IsNullOrEmpty(_applicationConfigurationsFilePath))
        {
            _logger.LogWarning($"{mainAssemblyName} cannot load file {mainAssemblyName}.env");
        }
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
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
