using DeviceAPI;
using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register necessary services
builder.Services.AddEndpointsApiExplorer(); // Needed for minimal API
builder.Services.AddSwaggerGen();           // Swagger support
builder.Services.AddAuthorization();        // For UseAuthorization middleware

var app = builder.Build();

// Use middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization(); // No auth logic yet — just scaffolded

// app.MapGet(...);  <-- You'll add endpoints here later

app.Run();