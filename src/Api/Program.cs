using Microsoft.AspNetCore.Mvc;
using Api;
using System;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using MotorController;
using Functions;
using Microsoft.Extensions.Logging;

// Start API server and define endpoints
public static class ApiHost
{
    // Start the API server with a shared HatchController instance
    public static void Start(HatchController hatch, string port)
    {
        // Setup web server
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders(); // Removes Console logger and others

        builder.WebHost.UseUrls($"https://0.0.0.0:{port}");
        var app = builder.Build();
        app.UseHttpsRedirection();

        // Root endpoint
        app.MapGet("/", () => "The API is online");

        // Get shared helpers
        Helper apiHelper = Helper.Instance;
        MainFunctions mainFunction = MainFunctions.Instance;

        // Endpoint: get newest image URL
        app.MapGet("/newest-url", () =>
        {
            try
            {
                // Return newest URL
                return Results.Ok(new { newestUrl = apiHelper.mNewestUrl });
            }
            catch (Exception ex)
            {
                Log("Api", $"Error updating or fetching newest URL: {ex.Message}");
                return Results.Problem($"Error updating or fetching newest URL: {ex.Message}");
            }
        });

        // Endpoint: update image URL
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

        // Endpoint: open/close hatch
        app.MapGet("/move-hatch", () =>
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

        // Endpoint: take picture
        app.MapGet("/take-picture", () =>
        {
            // Error checking: ensure handlePicture returns a bool and log result
            bool pictureSuccess = false;
            HandleError(() =>
            {
                Log("Api", "Taking picture");
                pictureSuccess = MainFunctions.Instance.handlePicture(); // Call singleton instance method
                if (pictureSuccess)
                {
                    Log("Api", "Picture taken");
                    bool success = HandleError(() =>
                    {
                        apiHelper.UpdateUrl(); // Update the URL using the API's Helper instance
                    });
                    if (!success)
                    {
                        Log("Api", "Failed to update URL");
                    }
                    else
                    {
                        Log("Api", $"Updated newest URL to {apiHelper.mNewestUrl}");
                    }
                }
                else
                {
                    Log("Api", "[ERROR] Picture not taken");
                }
            });
            // Return result with error checking
            return Results.Ok(new { success = pictureSuccess });
        });

        // Endpoint: stop hatch
        app.MapGet("/stop-hatch", () =>
        {
            HandleError(() =>
            {
                Log("Api", "Stopping hatch");
                hatch.StopHatch();
            });
            return Results.Ok(new { message = "Hatch stopped" });
        });

        // Endpoint: toggle detection
        app.MapGet("/toggle-detection", () =>
        {
            if (mainFunction.enableDetection)
            {
                Log("Api", "Disabling AI detection");
                mainFunction.enableDetection = false; // Disable detection
                return Results.Ok(new { message = "disabled" });
            }
            else
            {
                Log("Api", "Enabling AI detection");
                mainFunction.enableDetection = true; // Enable detection
                return Results.Ok(new { message = "enabled" });
            }
        });

        Log("Api", $"Trying to start API server on https://localhost:{port}");
        app.Run();
    }
}