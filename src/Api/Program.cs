using Microsoft.AspNetCore.Mvc; // For IActionResult
using Api;
using System;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://0.0.0.0:5000"); // Listen on static port 5000
var app = builder.Build();

app.MapGet("/", () => "The API is online");


Helper apiHelper = Helper.Instance; // Use the shared Helper instance


app.MapGet("/newest-url", () =>
{
    try
    {
        apiHelper.UpdateUrl(); // Update the URL to ensure it is fresh
        Log("Api", "Updated newest URL");
        return Results.Ok(new { newestUrl = apiHelper.mNewestUrl }); // Return the newest URL as JSON
    }
    catch (Exception ex)
    {
        Log("Api", $"Error updating or fetching newest URL: {ex.Message}");
        // Return error details if something goes wrong
        return Results.Problem($"Error updating or fetching newest URL: {ex.Message}");
    }
});

app.Run();

//TODO: Make the api accept commands to open the hatch, restart, ...
//! Implement auth + use cf tunnels