namespace server;

using System.Text.Json;
using Mqtt.Controllers;

public class IotController(ILogger<IotController> logger,
    MyDbContext db
) : MqttController
{
    [MqttRoute("station/tm_wt_station/sensor/+/telemetry")]
    public async Task ListenForMeasurements(Measurement m, string sensorId)
    {
        logger.LogInformation(JsonSerializer.Serialize(m));
        m.Id = Guid.NewGuid();
        await db.Measurements.AddAsync(m);
        await db.SaveChangesAsync();
    }
}