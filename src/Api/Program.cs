using Microsoft.AspNetCore.Mvc;
using Api;
using System;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using MotorController;
using static CameraFeed.CameraFeed;
using Functions;

public static class ApiHost
{
    // Start the API server with a shared HatchController instance
    public static void Start(HatchController hatch, string port)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"https://0.0.0.0:{port}");
        var app = builder.Build();

        app.MapGet("/", () => "The API is online");

        Helper apiHelper = Helper.Instance;

        app.MapGet("/newest-url", () =>
        {
            try
            {
                //apiHelper.UpdateUrl();
                //Log("Api", "Updated newest URL");
                return Results.Ok(new { newestUrl = apiHelper.mNewestUrl });
            }
            catch (Exception ex)
            {
                Log("Api", $"Error updating or fetching newest URL: {ex.Message}");
                return Results.Problem($"Error updating or fetching newest URL: {ex.Message}");
            }
        });

        app.MapGet("/update-url", () =>
        {
            bool success = HandleError(() =>
            {
                apiHelper.UpdateUrl(); // Update the URL using the API's Helper instance
            });
            if (!success)
            {
                Log("Api", "Failed to update URL via /update-url endpoint");
                return Results.Problem("Failed to update URL");
            }
            Log("Api", $"Updated newest URL to {apiHelper.mNewestUrl}");
            return Results.Ok(new { newestUrl = apiHelper.mNewestUrl });
        });

        app.MapGet("/open-hatch", () =>
        {
            HandleError(() =>
            {
                if (!hatch.hatchProperties.isOpen)
                {
                    Log("Api", "Opening hatch");
                    hatch.MoveHatch(true);
                }
                else
                {
                    Log("Api", "Closing hatch");
                    hatch.MoveHatch(false);
                }
            });
            return Results.Ok(new { isOpen = hatch.hatchProperties.isOpen });
        });

        app.MapGet("/take-picture", () =>
        {
            HandleError(() =>
            {
                Log("Api", "Taking picture");
                TakePicture();
            });
        });

        app.Run();
    }
}