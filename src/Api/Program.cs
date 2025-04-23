using Microsoft.AspNetCore.Mvc; // For IActionResult
using Api;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "The API is online");


Helper apiHelper = Helper.Instance; // Use the shared Helper instance


app.MapGet("/newest-url", () =>
{
    try
    {
        apiHelper.UpdateUrl(); // Update the URL to ensure it is fresh

        return Results.Ok(new { newestUrl = apiHelper.mNewestUrl }); // Return the newest URL as JSON
    }
    catch (Exception ex)
    {
        // Return error details if something goes wrong
        return Results.Problem($"Error updating or fetching newest URL: {ex.Message}");
    }
});

app.Run();
