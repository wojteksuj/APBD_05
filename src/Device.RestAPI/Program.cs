using DeviceAPI;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

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
var deviceService = new DeviceService(connectionString);


app.MapGet("/api/devices", () =>
{
    return Results.Ok(deviceService.GetAllDevices());
});




app.MapPost("/devices/smartwatches", (Smartwatch sw) =>
{
    deviceManager.AddDevice(sw);
    return Results.Ok(sw);
});

app.MapPost("/devices/embeddeddevices", (Embedded ed) =>
{
    deviceManager.AddDevice(ed);
    return Results.Ok(ed);
});

app.MapPost("/devices/pcs", (PersonalComputer pc) =>
{
    deviceManager.AddDevice(pc);
    return Results.Ok(pc);
});

app.MapGet("/devices/{id}", (String id) =>
{
    var device = deviceManager.GetDeviceById(id);
    return device != null ? Results.Ok(device) : Results.NotFound();
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