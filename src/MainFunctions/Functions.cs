namespace Functions;

using static CameraFeed.CameraFeed;
using Ntfy;
using System.Threading.Tasks;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

public class MainFunctions
{
    // Singleton instance
    public static readonly MainFunctions Instance = new MainFunctions();
    public string apiUrl = "https://localhost:8080";


    public void trySentNotification(string pImageUrl = "")
    {
        string msg = $"There was an package detected at your doorway, please check the app";
        // Define the actions header
        string actions = "view, Open App, smartpackagebox://open, clear=true;";

        // Run the async method synchronously and handle exceptions
        try
        {
            // Wait for the async method to complete
            Notifications.sendNotification(msg, "Package Detected!", actions, pImageUrl)
                .GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // Print any errors to the console
            Log("trySendNotification", $"Error sending notification: {ex.Message}");
        }
    }
    /*public bool handleUrl() //? Im using TriggerApiUpdateUrlAsync instead because it will always work and isnt as complicated with shared instances etc.
    {
        Helper apiHelper = Helper.Instance;
        bool success = HandleError(() =>
        {
            apiHelper.UpdateUrl(); //Update the photoUrl
        });
        if (!success)
        {
            Log("handleUrl", "[ERROR] Failed to update URL");
            return false;
        }
        else
        {
            Log("Function", $"Updated API URL {apiHelper.mNewestUrl}");
            return true;
        }
    }*/

    public async Task<bool> TriggerApiUpdateUrlAsync()
    {
        // Create HttpClient instance for sending HTTP requests
        using var client = new HttpClient();
        try
        {
            // Send GET request to the API's /update-url endpoint
            var response = await client.GetAsync($"{apiUrl}/update-url");

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Log("TriggerApiUpdateUrlAsync", $"UpdateUrl triggered successfully: {content}");
                return true;
            }
            else
            {
                Log("TriggerApiUpdateUrlAsync", $"Failed to trigger UpdateUrl. Status: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log("TriggerApiUpdateUrlAsync", $"Exception while triggering UpdateUrl: {ex.Message}");
            return false;
        }
    }
    public bool handlePicture()
    {
        byte[]? tImage = null;

        if (!File.Exists($"latest.png"))
        {
            Log("handlePicture", "latest.png does not exist.");
            // Create the file and immediately close the handle to avoid locking
            using (var fs = File.Create("./wwwroot/latest.png")) { }
        }

        bool success = HandleError(() =>
        {
            tImage = TakePicture();
        });

        if (!success || tImage == null)
        {
            Log("handlePicture", "[ERROR] Failed to take picture.");
            return false;
        }
        else
        {
            Log("handlePicture", "Took picture.");
        }

        if (tImage != null)
        {
            HandleError(() => SaveImage(tImage, "./wwwroot/latest.png"));
            //Log("handlePicture", "Saved image to latest.png");
        }
        else
        {
            Log("handlePicture", "[ERROR] Failed to save image to latest.png");
            return false;
        }
        return true;
    }
}