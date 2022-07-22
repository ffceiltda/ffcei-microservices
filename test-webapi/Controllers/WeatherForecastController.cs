using FFCEI.Microservices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Messages;

namespace Controllers;

[ApiExplorerSettings(GroupName = "Weather actions")]
[Route("[controller]")]
public class WeatherForecastController : WebApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get current weather
    /// </summary>
    /// <param name="city">city (optional)</param>
    /// <returns>Current weather</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecastResponse> Get([FromQuery] string? city = null)
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecastResponse
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    /// <summary>
    /// Put new weather
    /// </summary>
    /// <param name="request">New weather data</param>
    /// <returns>OK</returns>
    [HttpPut, Route("PutWeather")]
    public ActionResult Put([FromBody] WeatherForecastRequest request)
    {
        return Ok();
    }
}
