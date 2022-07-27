using EasyCaching.Core.Configurations;
using EasyCaching.Redis;
using System.Text;

namespace FFCEI.Microservices.Configuration;

/// <summary>
/// Redis Connection Configuration
/// </summary>
public sealed class RedisConnectionConfiguration : ConnectionConfiguration
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
    /// Database number
    /// </summary>
    public int Database { get; set; }

    /// <summary>
    /// Serializer name
    /// </summary>
    public string? SerializerName { get; set; }

    /// <summary>
    /// Connection name
    /// </summary>
    public string? ConnectionName { get; set; }

    /// <summary>
    /// Use SSL
    /// </summary>
    public bool Ssl { get; set; }

    /// <summary>
    /// Allow admin
    /// </summary>
    public bool AllowAdmin { get; set; } = true;

    /// <summary>
    /// Number of connection retries
    /// </summary>
    public int ConnectRetry { get; set; } = 9;

    /// <summary>
    /// Connection timeout
    /// </summary>
    public int ConnectTimeout { get; set; } = 10000;

    /// <summary>
    /// Keep-alive timeout
    /// </summary>
    public int KeepAliveTimeout { get; set; } = 60;

    /// <summary>
    /// Synchronous call timeout
    /// </summary>
    public int SyncTimeout { get; set; } = 120000;

    /// <summary>
    /// Asynchronous call timeout
    /// </summary>
    public int AsyncTimeout { get; set; } = 120000;

    protected override StringBuilder BuildConnectionString()
    {
        var stringBuilder = new StringBuilder();

#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA1305 // Specify IFormatProvider
        if (string.IsNullOrEmpty(Host))
        {
            throw new ArgumentNullException(nameof(Host));
        }

        stringBuilder.Append(Host);

        if ((Port is not null) && (Port.Value != 0))
        {
            stringBuilder.Append($":{Port}");
        }

        if (!string.IsNullOrEmpty(UserName))
        {
            stringBuilder.Append($",user={UserName}");
        }

        if (!string.IsNullOrEmpty(Password))
        {
            stringBuilder.Append($",password={Password}");
        }

        if (Database > 0)
        {
            stringBuilder.Append($",defaultDatabase={Database}");
        }

        if (!string.IsNullOrEmpty(ConnectionName))
        {
            stringBuilder.Append($",name={ConnectionName}");
        }

        stringBuilder.Append(",resolveDns=true");
        stringBuilder.Append($",ssl={Ssl}");
        stringBuilder.Append($",allowAdmin={AllowAdmin}");
        stringBuilder.Append($",connectRetry={ConnectRetry}");
        stringBuilder.Append($",connectTimeout={ConnectTimeout}");
        stringBuilder.Append($",keepAlive={KeepAliveTimeout}");
        stringBuilder.Append($",syncTimeout={SyncTimeout}");
        stringBuilder.Append($",asyncTimeout={AsyncTimeout}");
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore IDE0058 // Expression value is never used

        return stringBuilder;
    }

    /// <summary>
    /// Apply configuration to options
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException">throw if options is null</exception>
    /// <exception cref="InvalidOperationException">throw if Host or port is null</exception>
    public void Apply(RedisOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.SerializerName = SerializerName ?? "json";

        if (Host is null)
        {
            throw new InvalidOperationException("No Host specified in Redis configuration");
        }

        if (Port is null)
        {
            throw new InvalidOperationException("No Port specified in Redis configuration");
        }

        options.DBConfig.Endpoints.Add(new ServerEndPoint(Host, (int)Port));
        options.DBConfig.Username = UserName;
        options.DBConfig.Password = Password;
        options.DBConfig.Database = Database;
        options.DBConfig.AllowAdmin = AllowAdmin;
        options.DBConfig.ConnectionTimeout = ConnectTimeout;
        options.DBConfig.SyncTimeout = SyncTimeout;
        options.DBConfig.AsyncTimeout = AsyncTimeout;
    }
}
