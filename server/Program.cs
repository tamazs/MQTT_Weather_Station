using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using NSwag;
using NSwag.Generation.Processors.Security;
using server;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.GroupRealtime;
using Testcontainers.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionStrings = new ConnectionStrings();
configuration.GetSection(nameof(ConnectionStrings)).Bind(connectionStrings);
if (string.IsNullOrWhiteSpace(connectionStrings.DbConnectionString))
{
    var container = new PostgreSqlBuilder("postgres:15.1").Build();
    container.StartAsync().GetAwaiter().GetResult();
    connectionStrings.DbConnectionString = container.GetConnectionString();
}

builder.Services.AddSingleton(connectionStrings);

builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(0));

builder.Services.AddInMemorySseBackplane();
builder.Services.AddEfRealtime();
builder.Services.AddGroupRealtime();


builder.Services.AddDbContext<MyDbContext>((sp, conf) =>
{
    conf.UseNpgsql(connectionStrings.DbConnectionString);
    conf.AddEfRealtimeInterceptor(sp);
});
builder.Services.AddOpenApiDocument();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            context.ProblemDetails.Detail = exception.Message;
        }
    };
});
builder.Services.AddMqttControllers();
builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    db.Database.EnsureCreated();
}

app.UseExceptionHandler();
app.UseOpenApi();
app.UseSwaggerUi();
app.MapControllers();
app.UseStaticFiles();
app.UseCors(c => 
    c.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .SetIsOriginAllowed(_ => true));

 var mqttClient = app.Services.GetRequiredService<IMqttClientService>();
 Console.WriteLine(JsonSerializer.Serialize(connectionStrings));
 await mqttClient.ConnectAsync(connectionStrings.MqttBroker, 1883);
 app.GenerateApiClientsFromOpenApi("../client/src/generated-ts-client.ts", "./openapi.json").GetAwaiter().GetResult();

app.Run();