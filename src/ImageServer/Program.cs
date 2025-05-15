using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Server is up and running!");


if (CheckWwwRootExists())
{
    // Serve static files from wwwroot
    app.UseFileServer();
}
//TODO: Add Auth

// Run the application on localhost:8080
app.Run("http://localhost:8080");


bool CheckWwwRootExists()
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