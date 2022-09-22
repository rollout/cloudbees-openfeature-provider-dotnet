using Microsoft.AspNetCore.Mvc;
using OpenFeature.SDK;

namespace CloudBees.OpenFeature.Provider.Samples.AspNetCore.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly FeatureClient _flagClient;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, FeatureClient flagClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _flagClient = flagClient ?? throw new ArgumentNullException(nameof(flagClient));
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var showAsFahrenheit = await _flagClient.GetBooleanValue("ShowAsFahrenheit", false);
        _logger.LogInformation("Evaluated ShowAsFahrenheit flag as {Value}", showAsFahrenheit);
        return Enumerable.Range(1, 5).Select(index =>
            {
                var temperature = Random.Shared.Next(-20, 55);
                return new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    Temperature = showAsFahrenheit ? 32 + (int)(temperature / 0.5556) : temperature,
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
            })
            .ToArray();
    }
}