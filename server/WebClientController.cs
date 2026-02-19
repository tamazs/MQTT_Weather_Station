using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Mqtt.Controllers;

namespace server;

[ApiController]
[Route("api/[controller]")]
public class WebClientController(IMqttClientService mqtt) : ControllerBase
{
    [HttpPost("{sensorId}/command")]
    public async Task SendCommand(string sensorId, [FromBody] JsonElement command)
    {
        await mqtt.PublishAsync($"station/tm_wt_station/sensor/{sensorId}/command", command.GetRawText());
    }
}