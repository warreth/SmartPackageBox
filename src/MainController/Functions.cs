namespace Functions;
using MotorController;
using static CameraFeed.CameraFeed;
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

        //Take the picture + logging
        bool success;
        if (handlePicture())
        {
            Log("Function", "Picture taken and saved.");
            success = true;
        }

        else
        {
            Log("Function", "[ERROR] Failed to take picture.");
            success = false;
        }

        //Update the url
        if (success)
        {
            handleUrl();
        }


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
            Log("trySendNotification", $"Error sending notification: {ex.Message}");
        }
    }
    public bool handlePicture()
    {
        byte[]? tImage = null;

        if (!File.Exists($"latest.png"))
        {
            Log("handlePicture", "latest.png does not exist.");
            File.Create("latest.png");
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
            SaveImage(tImage, "latest.png");
            Log("handlePicture", "Saved image to latest.png");
        }
        else
        {
            Log("handlePicture", "[ERROR] Failed to save image to latest.png");
            return false;
        }
        return true;
    }
    public bool handleUrl()
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
    }
    public class StaticFunctions
    {

    }
}