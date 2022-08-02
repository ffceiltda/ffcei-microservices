using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Map model builders and keeps a cache for speed-up request processing
/// </summary>
public static class ModelBuilderMapper
{
    private static readonly Dictionary<string, List<MethodInfo>> _assembliesDecoded = new();

    /// <summary>
    /// Map model builders from a assembly
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder instance</param>
    /// <param name="databaseEngine">Database engine</param>
    /// <param name="initialAssembly">Initial assembly</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void MapModelBuilders(ModelBuilder modelBuilder, DatabaseEngine databaseEngine, Assembly? initialAssembly)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        if (initialAssembly is null)
        {
            return;
        }

        var modelAssemblyFullName = initialAssembly.FullName ?? string.Empty;

        if (string.IsNullOrEmpty(modelAssemblyFullName))
        {
            return;
        }

        List<MethodInfo>? methods = null;

        lock (_assembliesDecoded)
        {
            var _ = _assembliesDecoded.TryGetValue(modelAssemblyFullName, out methods);
        }

        if (methods is null)
        {
            var referencedAssemblies = initialAssembly.GetReferencedAssemblies();

            if (referencedAssemblies is null)
            {
                return;
            }

            var assemblies = new List<Assembly> { initialAssembly };

            foreach (var referencedAssembly in referencedAssemblies)
            {
                var assemblyName = referencedAssembly.Name ?? string.Empty;

                if (assemblyName.EndsWith(".Models", StringComparison.InvariantCulture))
                {
                    var newName = assemblyName.Replace(".Models", ".EntityFrameworkCore", StringComparison.InvariantCulture);
                    var assembly = Assembly.Load(newName);

                    if (assembly is not null)
                    {
                        assemblies.Add(assembly);
                    }
                }
            }

            methods = new();

            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();

                foreach (var assemblyType in assemblyTypes)
                {
                    if ((assemblyType.Namespace is not null) &&
                        (assemblyType.Namespace.EndsWith("ModelBuilders", StringComparison.InvariantCulture)) &&
                        (assemblyType.Name.EndsWith("ModelBuilder", StringComparison.InvariantCulture)))
                    {
                        var assemblyStaticMethod = assemblyType.GetMethod("OnModelCreating", BindingFlags.Public | BindingFlags.Static);

                        if (assemblyStaticMethod is not null)
                        {
                            methods.Add(assemblyStaticMethod);
                        }
                    }
                }
            }

            if (!_assembliesDecoded.ContainsKey(modelAssemblyFullName))
            {
                lock (_assembliesDecoded)
                {
                    if (!_assembliesDecoded.ContainsKey(modelAssemblyFullName))
                    {
                        _assembliesDecoded.Add(modelAssemblyFullName, methods);
                    }
                    else
                    {
                        if (_assembliesDecoded.TryGetValue(modelAssemblyFullName, out List<MethodInfo>? previousMethods))
                        {
                            foreach (var method in methods)
                            {
                                previousMethods.Add(method);
                            }

                            methods = previousMethods;
                        }
                    }
                }
            }
        }

        foreach (var method in methods)
        {
            var parameters = new object[2] { modelBuilder, databaseEngine };
            var _ = method.Invoke(null, parameters);
        }

        MapValueConverters(modelBuilder, databaseEngine);
    }

    private static void MapValueConverters(ModelBuilder modelBuilder, DatabaseEngine databaseEngine)
    {
        switch (databaseEngine)
        {
        case DatabaseEngine.Unknown:
            {
                break;
            }
        case DatabaseEngine.MySql:
            {
                MapValueConvertersMySql(modelBuilder);

                break;
            }
        default:
            {
                break;
            }
        }
    }

    private static void MapValueConvertersMySql(ModelBuilder modelBuilder)
    {
        var properties = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).ToList();

        foreach (var property in properties.Where(p => p.ClrType == typeof(DateTimeOffset)))
        {
            property.SetValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
                convertToProviderExpression: source => source.UtcDateTime,
                convertFromProviderExpression: source => new DateTimeOffset(source, TimeSpan.Zero)
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(DateTimeOffset?)))
        {
            property.SetValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
                convertToProviderExpression: source => source == null ? null : source.Value.UtcDateTime,
                convertFromProviderExpression: source => source == null ? null : new DateTimeOffset(source.Value, TimeSpan.Zero)
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(TimeSpan)))
        {
            property.SetValueConverter(new ValueConverter<TimeSpan, long>(
                convertToProviderExpression: source => source.Ticks,
                convertFromProviderExpression: source => TimeSpan.FromTicks(source)
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(TimeSpan?)))
        {
            property.SetValueConverter(new ValueConverter<TimeSpan?, long?>(
                convertToProviderExpression: source => source == null ? null : source.Value.Ticks,
                convertFromProviderExpression: source => source == null ? null : TimeSpan.FromTicks(source.Value)
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(bool)))
        {
            property.SetValueConverter(new ValueConverter<bool, long>(
                convertToProviderExpression: source => source == true ? 1 : 0,
                convertFromProviderExpression: source => source == 0 ? false : true
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(bool?)))
        {
            property.SetValueConverter(new ValueConverter<bool?, long?>(
                convertToProviderExpression: source => source == null ? null : source.Value == true ? 1 : 0,
                convertFromProviderExpression: source => source == null ? null : source.Value == 0 ? false : true
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(ushort)))
        {
            property.SetValueConverter(new ValueConverter<ushort, long>(
                convertToProviderExpression: source => (long)source,
                convertFromProviderExpression: source => (ushort)source
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(ushort?)))
        {
            property.SetValueConverter(new ValueConverter<ushort?, long?>(
                convertToProviderExpression: source => source == null ? null : (long)source.Value,
                convertFromProviderExpression: source => source == null ? null : (ushort)source.Value
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(uint)))
        {
            property.SetValueConverter(new ValueConverter<uint, long>(
                convertToProviderExpression: source => (long)source,
                convertFromProviderExpression: source => (uint)source
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(uint?)))
        {
            property.SetValueConverter(new ValueConverter<uint?, long?>(
                convertToProviderExpression: source => source == null ? null : (long)source.Value,
                convertFromProviderExpression: source => source == null ? null : (uint)source.Value
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(ulong)))
        {
            property.SetValueConverter(new ValueConverter<ulong, long>(
                convertToProviderExpression: source => (long)source,
                convertFromProviderExpression: source => (ulong)source
            ));
        }

        foreach (var property in properties.Where(p => p.ClrType == typeof(ulong?)))
        {
            property.SetValueConverter(new ValueConverter<ulong?, long?>(
                convertToProviderExpression: source => source == null ? null : (long)source.Value,
                convertFromProviderExpression: source => source == null ? null : (ulong)source.Value
            ));
        }
    }
}
