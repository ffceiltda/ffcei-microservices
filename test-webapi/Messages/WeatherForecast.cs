using FFCEI.Microservices.AspNetCore;

namespace Navi.Backend.RegistrationService.Messages;

/// <summary>
/// Weather forecast response
/// </summary>
[SwaggerMessage]
public class WeatherForecastRequest
{
    /// <summary>
    /// Timestamp of message
    /// </summary>
    [SwaggerProperty]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Degress Celsius
    /// </summary>
    [SwaggerProperty]
    public int TemperatureC { get; set; }

    /// <summary>
    /// Summary
    /// </summary>
    [SwaggerProperty]
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
    [SwaggerProperty]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Degress Celsius
    /// </summary>
    [SwaggerProperty]
    public int TemperatureC { get; set; }

    /// <summary>
    /// Degrees Farenheit
    /// </summary>
    [SwaggerProperty]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Summary
    /// </summary>
    [SwaggerProperty]
    public string? Summary { get; set; }
}
