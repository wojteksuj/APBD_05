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








app.Run();