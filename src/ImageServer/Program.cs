using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using Microsoft.Extensions.Logging;

public static class ImageServer
{
    public static void Start(string port)
    {
        if (string.IsNullOrEmpty(port))
        {
            Log("ImageServer", "Port is null or empty. Defaulting to \"8081\".");
            port = "8081";
        }

        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        var app = builder.Build();
        //app.UseHttpsRedirection();

        app.MapGet("/", () => "Server is up and running!");

        string contentRootPath = builder.Environment.ContentRootPath;
        string wwwRootPath = Path.Combine(contentRootPath, "wwwroot");

        if (!Directory.Exists(wwwRootPath))
        {
            Log("ImageServerSetup", $"wwwroot directory does not exist at '{wwwRootPath}'. Creating it.");
            try
            {
                Directory.CreateDirectory(wwwRootPath);
                Log("ImageServerSetup", $"Created wwwroot directory at '{wwwRootPath}'.");
            }
            catch (Exception ex)
            {
                Log("ImageServerSetup", $"[ERROR] Failed to create wwwroot directory at '{wwwRootPath}': {ex.Message}");
            }
        }
        else
        {
            Log("ImageServerSetup", $"wwwroot directory already exists at '{wwwRootPath}'.");
        }

        app.UseStaticFiles();
        Log("ImageServer", $"Trying to start ImageServer on http://localhost:{port}");
        app.Run();
    }
}
