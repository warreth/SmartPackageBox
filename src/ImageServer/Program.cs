using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

public static class ImageServer
{
    public static void Start(string port)
    {
        // Error checking: ensure port is not null or empty
        if (string.IsNullOrEmpty(port))
        {
            Log("ImageServer", "Port is null or empty. Defaulting to \"8080\".");
            port = "8080";
        }

        // Use Array.Empty<string>() instead of undefined 'args' (library context) //!Thank you GPT-4.1 because i couldnt find the error at all.
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var app = builder.Build();

        app.MapGet("/", () => "Server is up and running!");

        // Check if wwwroot exists, create if not
        if (CheckWwwRootExists())
        {
            // Serve static files from wwwroot
            app.UseFileServer();
        }

        // Run the application on the specified port
        app.Run($"http://localhost:{port}");
        Log("ImageServer", $"Server started on http://localhost:{port}");
    }

    public static bool CheckWwwRootExists()
    {
        if (!Directory.Exists("wwwroot"))
        {
            Log("WwwRootExists", "wwwroot directory does not exist.");
            Directory.CreateDirectory("wwwroot");
            Log("WwwRootExists", "Created wwwroot directory. (not error checked)");
            return false;
        }
        return true;
    }
}