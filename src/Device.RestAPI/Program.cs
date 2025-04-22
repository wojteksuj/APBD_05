using System.Text.Json;
using System.Text.Json.Nodes;
using DeviceAPI;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Can't connect to database, wrong connection string");
}
builder.Services.AddSingleton<IDeviceService, DeviceService>(deviceService => new DeviceService(connectionString));

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           
builder.Services.AddAuthorization();      

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

var deviceManager = new DeviceManager();


app.MapGet("/api/devices", (IDeviceService deviceService) =>
{
    try
    {
        return Results.Ok(deviceService.GetAllDevices());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
    
});

app.MapPost("/api/devices", async (IDeviceService deviceService, HttpRequest request) =>
    {
        string? contentType = request.ContentType?.ToLower();

        switch (contentType)
        {
            case "application/json":
            {
                using var reader = new StreamReader(request.Body);
                string rawJson = await reader.ReadToEndAsync();

                var json = JsonNode.Parse(rawJson);
                if (json is null) return Results.BadRequest("Invalid JSON");

                var type = json["type"]?.ToString()?.ToLower();
                if (string.IsNullOrEmpty(type))
                    return Results.BadRequest("Missing 'type' field.");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Device? device = type switch
                {
                    "smartwatch" => JsonSerializer.Deserialize<Smartwatch>(rawJson, options),
                    "pc"         => JsonSerializer.Deserialize<PersonalComputer>(rawJson, options),
                    "embedded"   => JsonSerializer.Deserialize<Embedded>(rawJson, options)
                };

                if (device is null)
                    return Results.BadRequest($"Unsupported device type: {type}");

                deviceService.AddDevice(device); 
                return Results.Created($"/api/devices/{device.Id}", device);
            }

            case "text/plain":
                return Results.Ok();

            default:
                return Results.Conflict();
        }
    })
    .Accepts<string>("application/json", ["text/plain"]);

app.MapGet("/api/devices/{id}", (string id, IDeviceService service) =>
{
    var device = service.GetDeviceById(id);
    return Results.Ok(device);
});










app.MapDelete("devices/delete/{id}", (String id) =>
{
    var device = deviceManager.GetDeviceById(id);
    if (device == null) return Results.NotFound();
    deviceManager.RemoveDeviceById(id);
    return Results.Ok();
});

app.MapPut("devices/smartwatches/{id}", (String id, Smartwatch editSw) =>
{
    var existingDevice = deviceManager.GetDeviceById(id);
    if (existingDevice is Smartwatch)
    {
        deviceManager.EditDevice(editSw);
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/pcs/{id}", (String id, PersonalComputer editPc) =>
{
    var existingDevice = deviceManager.GetDeviceById(id);
    if (existingDevice is PersonalComputer)
    {
        deviceManager.EditDevice(editPc);
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/embeddeddevices/{id}", (String id, Embedded editEd) =>
{
    var existingDevice = deviceManager.GetDeviceById(id);
    if (existingDevice is Embedded)
    {
        deviceManager.EditDevice(editEd);
        return Results.Ok();
    }
    return Results.NotFound();
});


app.Run();