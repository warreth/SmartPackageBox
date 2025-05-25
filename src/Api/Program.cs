using Microsoft.AspNetCore.Mvc;
using Api;
using System;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using MotorController;
using Functions;
using Microsoft.Extensions.Logging;

public static class ApiHost
{
    // Start the API server with a shared HatchController instance
    public static void Start(HatchController hatch, string port)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders(); // Removes Console logger and others

        builder.WebHost.UseUrls($"https://0.0.0.0:{port}");
        var app = builder.Build();
        app.UseHttpsRedirection();

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

        app.MapGet("/stop-hatch", () =>
        {
            HandleError(() =>
            {
                Log("Api", "Stopping hatch");
                hatch.StopHatch();
            });
            return Results.Ok(new { message = "Hatch stopped" });
        });

        Log("Api", $"Trying to start API server on https://localhost:{port}");
        app.Run();
    }
}