using System.Text.Json;
using System.Text.Json.Nodes;
using DeviceAPI;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyDatabase");
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
        
        using var reader = new StreamReader(request.Body);
        string rawJson = await reader.ReadToEndAsync();

        var json = JsonNode.Parse(rawJson);
        if (json is null)
            return Results.BadRequest("Invalid JSON");

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
            "embedded"   => JsonSerializer.Deserialize<Embedded>(rawJson, options),
            _            => null
        };

        if (device is null)
            return Results.BadRequest();

        bool success = deviceService.AddDevice(device);
        if (!success)
            return Results.Problem();

        return Results.Created();
    })
    .Accepts<string>("application/json", ["text/plain"]);


app.MapGet("/api/devices/{id}", (string id, IDeviceService service) =>
{
    var device = service.GetDeviceById(id);
    if(device is null) return Results.NotFound();
    return Results.Ok(device);
});

app.MapDelete("/api/devices/{id}", (string id, IDeviceService service) =>
{
    bool deleted = service.RemoveDevice(id);
    return deleted ? Results.Ok() : Results.NotFound();
});

app.MapPut("/api/devices/{id}", async (string id, HttpRequest request, IDeviceService service) =>
{
    using var reader = new StreamReader(request.Body);
    string rawJson = await reader.ReadToEndAsync();
    var json = JsonNode.Parse(rawJson);
    if (json is null) return Results.BadRequest("Invalid JSON file!!!");

    var type = json["type"]?.ToString()?.ToLower();
    
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    Device? device = type switch
    {
        "smartwatch"      => JsonSerializer.Deserialize<Smartwatch>(rawJson, options),
        "personalcomputer"=> JsonSerializer.Deserialize<PersonalComputer>(rawJson, options),
        "embeddeddevice"  => JsonSerializer.Deserialize<Embedded>(rawJson, options)
    };

    if (device is null || device.Id != id)
        return Results.BadRequest("Invalid device data!!!");

    bool updated = service.UpdateDevice(device);
    if(updated) return Results.Ok();
    return Results.NotFound();
});

app.Run();