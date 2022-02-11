using ProtoBuf.Grpc;
using System.Threading.Tasks;
using BlazorGrpcWebCodeFirst.Shared;
using System;
using System.Linq;

namespace BlazorGrpcWebCodeFirst.Server.Services;

#region snippet
public class WeatherService : IWeatherForecasts
{

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    public Task<WeatherForecasts> Get()
    {
        var rng = new Random();
        var temps = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();

        var data =  new WeatherForecasts()
        {
            Data = temps
        };
        return Task.FromResult(data);
    }
    
}
#endregion

