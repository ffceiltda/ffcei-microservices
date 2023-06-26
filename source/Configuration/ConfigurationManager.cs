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
                if (configurationSearchUserName.IndexOf(null) == -1)
                {
                    configurationSearchUserName.Insert(0, null);
                }
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            (var machinePath, var machineUserName) = TryLoadEnvironmentFromRegistry(Registry.LocalMachine);

            if (InsertDirectoryInSearchPath(ref configurationSearchPath, machinePath))
            {
                if (configurationSearchUserName.IndexOf(machineUserName) == -1)
                {
                    configurationSearchUserName.Insert(0, machineUserName);
                }
            }

            (var userPath, var userUserName) = TryLoadEnvironmentFromRegistry(Registry.CurrentUser);

            if (InsertDirectoryInSearchPath(ref configurationSearchPath, userPath))
            {
                if (configurationSearchUserName.IndexOf(userUserName) == -1)
                {
                    configurationSearchUserName.Insert(0, userUserName);
                }
            }
        }
        
        var machineSearchPath = Microservice.Instance?.ConfigurationMachineSearchPath;

        if (!string.IsNullOrEmpty(machineSearchPath))
        {
            if (Directory.Exists(machineSearchPath))
            {
                var _ = InsertDirectoryInSearchPath(ref configurationSearchPath, machineSearchPath);
            }
        }
        
        var userSearchPath = Microservice.Instance?.ConfigurationUserSearchPath;

        if (!string.IsNullOrEmpty(userSearchPath))
        {
            if (Directory.Exists(userSearchPath))
            {
                var _ = InsertDirectoryInSearchPath(ref configurationSearchPath, userSearchPath);
            }
        }

        TryLoadEnvironmentFromPath(mainAssemblyName, configurationSearchPath, configurationSearchUserName);
    }

    private static bool InsertDirectoryInSearchPath(ref List<string> searchPaths, string? path, bool append = false)
    {
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path) && (searchPaths.IndexOf(path) == -1))
        {
            if (append)
            {
                searchPaths.Add(path);
            }
            else
            {
                searchPaths.Insert(0, path);
            }

            return true;
        }

        return false;
    }

    private static (string? registryPath, string? registryUserName) TryLoadEnvironmentFromRegistry(RegistryKey registryKey)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var registryPathForConfiguration = Microservice.Instance?.RegistryPathForConfiguration;

            if (!string.IsNullOrEmpty(registryPathForConfiguration))
            {
                try
                {
                    string? registryPath = null;
                    string? registryUserName = null;

                    using (var key = registryKey.OpenSubKey(registryPathForConfiguration))
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

    private void TryLoadEnvironmentFromPath(string mainAssemblyName, List<string> searchPaths, List<string?> searchUserNames)
    {
        var existingPaths = new List<string>();

        foreach (var searchPath in searchPaths)
        {
            if (!Directory.Exists(searchPath))
            {
                continue;
            }

            var environmentPath = Path.Combine(searchPath, "Environment");
            var environmentRuntimePath = string.Empty;

            if (_isDevelopment)
            {
                environmentRuntimePath = Path.Combine(environmentPath, "Development");
            }
            else if (_isProduction)
            {
                environmentRuntimePath = Path.Combine(environmentPath, "Production");
            }

            foreach (var searchUserName in searchUserNames)
            {
                var userNameToCombine = string.IsNullOrEmpty(searchUserName) ? Environment.UserName : searchUserName;

                var userEnvironmentRuntimePath = Path.Combine(environmentRuntimePath, userNameToCombine);
                var userEnvironmentPath = Path.Combine(environmentPath, userNameToCombine);
                var userSearchPath = Path.Combine(searchPath, userNameToCombine);

#pragma warning disable IDE0058 // Expression value is never used
                InsertDirectoryInSearchPath(ref existingPaths, userEnvironmentRuntimePath, true);
                InsertDirectoryInSearchPath(ref existingPaths, environmentRuntimePath, true);
                InsertDirectoryInSearchPath(ref existingPaths, userEnvironmentPath, true);
                InsertDirectoryInSearchPath(ref existingPaths, environmentPath, true);
                InsertDirectoryInSearchPath(ref existingPaths, userSearchPath, true);
                InsertDirectoryInSearchPath(ref existingPaths, searchPath, true);
            }

            InsertDirectoryInSearchPath(ref existingPaths, environmentRuntimePath, true);
            InsertDirectoryInSearchPath(ref existingPaths, environmentPath, true);
            InsertDirectoryInSearchPath(ref existingPaths, searchPath, true);
#pragma warning restore IDE0058 // Expression value is never used
        }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
        if (System.Diagnostics.Debugger.IsAttached || _isDevelopment)
        {
            _logger.LogInformation($"Configuration search path: {existingPaths.Count}");

            foreach (var existingPath in existingPaths)
            {
                _logger.LogInformation(existingPath);
            }
        }
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        foreach (var existingPath in existingPaths)
        {
            if (!string.IsNullOrEmpty(_allConfigurationsFilePath) &&
                !string.IsNullOrEmpty(_applicationConfigurationsFilePath))
            {
                break;
            }

            if (string.IsNullOrEmpty(_allConfigurationsFilePath))
            {
                var allConfigurationsFilePath = Path.Combine(existingPath, "ALL.env");

                if (File.Exists(allConfigurationsFilePath))
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
                        if (System.Diagnostics.Debugger.IsAttached || _isDevelopment)
                        {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                            _logger.LogInformation($"Trying to load system-wide configurations from {allConfigurationsFilePath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                        }

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
                else if (System.Diagnostics.Debugger.IsAttached || _isDevelopment)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogWarning($"File ALL.env not found in {existingPath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }

            if (string.IsNullOrEmpty(_applicationConfigurationsFilePath))
            {
                var applicationConfigurationsFilePath = Path.Combine(existingPath, $"{mainAssemblyName}.env");

                if (File.Exists(applicationConfigurationsFilePath))
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
                        if (System.Diagnostics.Debugger.IsAttached || _isDevelopment)
                        {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                            _logger.LogInformation($"Trying to load application-specific configurations from {applicationConfigurationsFilePath} ...");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                        }

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
                else if (System.Diagnostics.Debugger.IsAttached || _isDevelopment)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogWarning($"File {mainAssemblyName}.env not found in {existingPath} ...");
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
