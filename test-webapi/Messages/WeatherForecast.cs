using FFCEI.Microservices.AspNetCore;

namespace TestWebApi.Messages;

/// <summary>
/// Weather forecast response
/// </summary>
[SwaggerRequest]
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
    public string? Summary { get; set; }
}

/// <summary>
/// Weather forecast response
/// </summary>
public class WeatherForecastResponse
{
    /// <summary>
    /// Timestamp of message
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Degress Celsius
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Degrees Farenheit
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Summary
    /// </summary>
    public string? Summary { get; set; }
}
