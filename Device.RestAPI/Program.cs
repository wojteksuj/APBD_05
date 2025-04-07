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
    var targetIndex = -1;
    for (var i = 0; i < devices.Count; i++)
    {
        var device = devices[i];
        if (device.Id.Equals(editSw.Id))
        {
            targetIndex = i;
            break;
        }
    }
    if (targetIndex == -1) return Results.NotFound();

    if (devices[targetIndex] is Smartwatch)
    {
        devices[targetIndex] = editSw;
    }
    else return Results.NotFound();
    return Results.Ok(editSw);
});

app.MapPut("devices/pc/update", (PersonalComputer editPc) =>
{
    var targetIndex = -1;
    for (var i = 0; i < devices.Count; i++)
    {
        var device = devices[i];
        if (device.Id.Equals(editPc.Id))
        {
            targetIndex = i;
            break;
        }
    }
    if (targetIndex == -1) return Results.NotFound();

    if (devices[targetIndex] is Smartwatch)
    {
        devices[targetIndex] = editPc;
    }
    else return Results.NotFound();
    return Results.Ok(editPc);
});

app.MapPut("devices/embedded/update", (Embedded editEd) =>
{
    var targetIndex = -1;
    for (var i = 0; i < devices.Count; i++)
    {
        var device = devices[i];
        if (device.Id.Equals(editEd.Id))
        {
            targetIndex = i;
            break;
        }
    }
    if (targetIndex == -1) return Results.NotFound();

    if (devices[targetIndex] is Smartwatch)
    {
        devices[targetIndex] = editEd;
    }
    else return Results.NotFound();
    return Results.Ok(editEd);
});

app.Run();