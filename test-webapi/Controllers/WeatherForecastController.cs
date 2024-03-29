using FFCEI.Microservices.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Messages;

namespace TestWebApi.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(GroupName = "Weather actions")]
[Route("[controller]")]
public class WeatherForecastController : WebApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
        : base(logger)
    {
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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Put([FromBody] WeatherForecastRequest request)
    {
        return WebApiResultWith<string>.Succeeded("OK, it works").ToHttpResponseAsResult();
    }
}
