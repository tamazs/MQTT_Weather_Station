using System.Text.Json;
using Mqtt.Controllers;

namespace server;

public class WeatherStationController(ILogger<WeatherStationController> logger) : MqttController
{
    [MqttRoute("station/tm_wt_station/sensor/{sensorId}/telemetry")]
    public async Task HandleTelemetry(string sensorId, SensorTelemetry data)
    {
        logger.LogInformation("Sensor: "+sensorId+", data: "+JsonSerializer.Serialize(data));
    }
}

public record SensorTelemetry(
    string SensorId,
    string SensorName,
    string StationId,
    DateTime Timestamp,
    double Temperature,
    double Humidity,
    double Pressure,
    int LightLevel,
    string Status);