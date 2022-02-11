using System.Runtime.Serialization;
using System.ServiceModel;

namespace BlazorGrpcWebCodeFirst.Shared;

[DataContract]
public class WeatherForecast
{
	[DataMember(Order = 1)]
	public DateTime Date { get; set; }

	[DataMember(Order = 2)]
	public int TemperatureC { get; set; }

	[DataMember(Order = 3)]
	public string? Summary { get; set; }

	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

[DataContract]
public class WeatherForecasts
{
	[DataMember(Order = 10)]
	public WeatherForecast[] Data {get; set;} = Array.Empty<WeatherForecast>();
}


[ServiceContract]
public interface IWeatherForecasts
{
	[OperationContract]
	Task<WeatherForecasts> Get();

}
