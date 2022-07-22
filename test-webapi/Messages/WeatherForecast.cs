using FFCEI.Microservices.AspNetCore;

namespace Messages;

/// <summary>
/// Weather forecast response
/// </summary>
[SwaggerMessage]
public class WeatherForecastRequest
{
    /// <summary>
    /// Timestamp of message
    /// </summary>
    [SwaggerRequiredProperty]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Degress Celsius
    /// </summary>
    [SwaggerRequiredProperty]
    public int TemperatureC { get; set; }

    /// <summary>
    /// Summary
    /// </summary>
    [SwaggerRequiredProperty]
    public string? Summary { get; set; }
}

/// <summary>
/// Weather forecast response
/// </summary>
[SwaggerMessage]
public class WeatherForecastResponse
{
    /// <summary>
    /// Timestamp of message
    /// </summary>
    [SwaggerRequiredProperty]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Degress Celsius
    /// </summary>
    [SwaggerRequiredProperty]
    public int TemperatureC { get; set; }

    /// <summary>
    /// Degrees Farenheit
    /// </summary>
    [SwaggerRequiredProperty]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Summary
    /// </summary>
    [SwaggerRequiredProperty]
    public string? Summary { get; set; }
}
