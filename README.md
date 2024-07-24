# CloudBees Feature Management provider for OpenFeature

This is the [CloudBees](https://www.cloudbees.com/products/feature-management) provider implementation for [OpenFeature](https://openfeature.dev/) for the [dotnet SDK](https://github.com/open-feature/dotnet-sdk).

OpenFeature provides a vendor-agnostic abstraction layer on Feature Flag management.

This provider allows the use of CloudBees Feature Management as a backend for Feature Flag configurations.

## Requirements
- netframework 4.5.1 or greater
- .net core 3.1 or greater

### Configuration

Follow the instructions on the [dotnet SDK project](https://github.com/open-feature/dotnet-sdk) for how to use the dotnet SDK.

You can configure the CloudBees provider by doing the following:


Console app
```csharp
using CloudBees.OpenFeature.Provider;
using OpenFeatureSDK = OpenFeature.SDK.OpenFeature;

var options = new CloudBeesOptions{ ApiKey = API_KEY_GOES_HERE };
await OpenFeatureSDK.Instance.SetProviderAsync(new CloudBeesProvider(options));
var client = OpenFeatureSDK.Instance.GetClient();
var myCoolFlag = await client.GetBooleanValue("My_Cool_Flag", false);
```

AspNetCore app
```csharp
// Add to service collection/DI setup
builder.Services.AddOpenFeatureCloudBees(options =>
{
    options.ApiKey = builder.Configuration.GetValue<string>("CloudBeeApiKey");
});

// Add to class
public class WeatherForecastController : ControllerBase
{
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
```

Thank you to [Ben Evenson](https://github.com/benjiro) for the initial implementation
