using System.Security.Cryptography;
using DeviceAPI;
using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.MapPost("/devices/smartwatch", (Smartwatch sw) =>
{
    deviceManager.AddDevice(sw);
    return Results.Ok(sw);
});

app.MapPost("/devices/embedded", (Embedded ed) =>
{
    deviceManager.AddDevice(ed);
    return Results.Ok(ed);
});

app.MapPost("/devices/pc", (PersonalComputer pc) =>
{
    deviceManager.AddDevice(pc);
    return Results.Ok(pc);
});

app.MapGet("/devices", () =>
{
    return Results.Ok(deviceManager.ShowAllDevices());
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

app.MapPut("devices/smartwatch/{id}", (String id, Smartwatch editSw) =>
{
    var existingDevice = deviceManager.GetDeviceById(id);
    if (existingDevice is Smartwatch)
    {
        deviceManager.EditDevice(editSw);
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/pc/{id}", (String id, PersonalComputer editPc) =>
{
    var existingDevice = deviceManager.GetDeviceById(id);
    if (existingDevice is PersonalComputer)
    {
        deviceManager.EditDevice(editPc);
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/embedded/{id}", (String id, Embedded editEd) =>
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