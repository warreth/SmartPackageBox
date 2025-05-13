namespace Functions;
using MotorController;
using CameraFeed;
using Ntfy;
using System.Threading.Tasks;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using Api;

public class Function
{
    HatchController hatch = new();

    public void packageIsDetected()
    {
        Log("Function", "Package is detected");

        // Take a picture and handle any errors, ensuring the program doesn't crash
        byte[]? tImage = null;
        bool success = HandleError(() =>
        {
            tImage = CameraFeed.TakePicture();
        });

        if (!success || tImage == null)
        {
            Log("Function", "Failed to take picture.");
        }

        if (File.Exists($"latest.png") && tImage != null)
        {
            CameraFeed.SaveImage(tImage, "latest.png");
            Log("Function", "Saved image to latest.png");
        }

        Helper apiHelper = Helper.Instance;
        apiHelper.UpdateUrl(); //Update the photoUrl
        Log("Function", $"Updated API URL {apiHelper.mNewestUrl}");

        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
        else
        {
            Log("Function", "Hatch is already open");
        }

        trySentNotification("https://yavuzceliker.github.io/sample-images/image-9.jpg");
        Thread.Sleep(10000); //Wait 10 seconds (10000 milliseconds
    }

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
            Log("Function", $"Error sending notification: {ex.Message}");
        }
    }

    public class StaticFunctions
    {

    }
}