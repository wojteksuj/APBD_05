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

var devices = new List<Device>();

app.MapPost("/devices/smartwatch", (Smartwatch sw) =>
{
    devices.Add(sw);
    return Results.Ok(sw);
});

app.MapPost("/devices/embedded", (Embedded ed) =>
{
    devices.Add(ed);
    return Results.Ok(ed);
});

app.MapPost("/devices/pc", (PersonalComputer pc) =>
{
    devices.Add(pc);
    return Results.Ok(pc);
});

app.MapGet("/devices", () =>
{
    return Results.Ok(devices);
});

app.MapGet("/devices/{id}", (String id) =>
{
    var device = devices.FirstOrDefault(d => d.Id == id);
    return device != null ? Results.Ok(device) : Results.NotFound();
});

app.MapDelete("devices/delete/{id}", (String id) =>
{
    var device = devices.FirstOrDefault(d => d.Id == id);
    if (device == null) return Results.NotFound();
    devices.Remove(device);
    return Results.Ok();
});

app.MapPut("devices/smartwatch/update", (Smartwatch editSw) =>
{
    var existingDevice = devices.FirstOrDefault(d => d.Id == editSw.Id);
    if (existingDevice is Smartwatch sw)
    {
        sw.Name = editSw.Name;
        sw.IsEnabled = editSw.IsEnabled;
        sw.BatteryLevel = editSw.BatteryLevel;
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/pc/update", (PersonalComputer editPc) =>
{
    var existingDevice = devices.FirstOrDefault(d => d.Id == editPc.Id);
    if (existingDevice is PersonalComputer pc)
    {
        pc.Name = editPc.Name;
        pc.IsEnabled = editPc.IsEnabled;
        pc.OperatingSystem = editPc.OperatingSystem;
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("devices/embedded/update", (Embedded editEd) =>
{
    var existingDevice = devices.FirstOrDefault(d => d.Id == editEd.Id);
    if (existingDevice is Embedded ed)
    {
        ed.Name = editEd.Name;
        ed.IsEnabled = editEd.IsEnabled;
        ed.IpAddress = editEd.IpAddress;
        ed.NetworkName = editEd.NetworkName;
        return Results.Ok();
    }
    return Results.NotFound();
});


app.Run();