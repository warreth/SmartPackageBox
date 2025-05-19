using AI;
using Functions;
using MotorController;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using Api;
using Emgu.CV;

public class Program
{
    public static string imageServerUrl = "https://localhost:8081";
    public static string apiUrl = "https://localhost:8080";



    public static void Main(string[] args)
    {
        HatchController sharedHatch = new();
        AiHelper aiHelper = new();
        Helper.BaseUrl = apiUrl;

        MainFunctions mainFunction = new(apiUrl);

        // Start API in a background thread so it doesn't block the main loop
        Task.Run(() => ApiHost.Start(sharedHatch, "8080")); //TODO: Fix that it blocks all other code from running
        Task.Run(() => ImageServer.Start("8081"));

        // Infinite loop to continuously check for package detection
        while (true)
        {
            if (aiHelper.detectPackageAI())
            {
                packageIsDetected(mainFunction, sharedHatch);
            }
            else
            {
                Log("Program", "Package is not detected");
            }
        }
    }
    public static void packageIsDetected(MainFunctions mainFunction, HatchController hatch)
    {
        Log("HelperFunctions", "Package is detected");

        //Take the picture + logging
        bool success;
        if (CameraFunctions.Instance.handlePicture())
        {
            Log("HelperFunctions", "Picture taken and saved.");
            success = true;
        }

        else
        {
            Log("HelperFunctions", "[ERROR] Failed to take picture.");
            success = false;
        }

        //Update the url
        if (success)
        {
            mainFunction.TriggerApiUpdateUrlAsync();
        }


        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
        else
        {
            Log("HelperFunctions", "Hatch is already open");
        }

        mainFunction.trySentNotification($"{imageServerUrl}/latest.png");
        Thread.Sleep(10000); //Wait 10 seconds (10000 milliseconds
    }
}