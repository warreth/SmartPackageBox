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
        Helper.BaseUrl = apiUrl;

        MainFunctions mainFunction = MainFunctions.Instance;

        // Start API in a background thread so it doesn't block the main loop
        Task.Run(() => ApiHost.Start(sharedHatch, "8080"));
        Task.Run(() => ImageServer.Start("8081"));

        string imagePath = "./wwwroot/latest.png";
        // Infinite loop to continuously check for package detection
        while (true)
        {
            if (mainFunction.enableDetection)
            {
                mainFunction.handlePicture(imagePath);

                if (HandleAIDetection(imagePath).GetAwaiter().GetResult()) // Wait for the AI detection to complete
                {
                    Log("Program", "Package is detected");

                    packageIsDetected(mainFunction, sharedHatch);
                }
                else
                {
                    Log("Program", "No package detected");
                }
            }
            else
            {
                Log("Program", "Package detection is disabled");
            }
            Thread.Sleep(5000); // Wait for 10 seconds before the next check
        }
    }
    public static async Task<bool> HandleAIDetection(string imagePath, bool isLocal = true)
    {
        string url = string.Empty;
        string ImagePath = imagePath;
        string ModelId = "packagedetection-xyah9";
        string ModelVersion = "3";

        if (isLocal)
        {
            url = AiHelper.RoboflowLocalBaseUrl;
            //Log("HandleAIDetection", "Using Local Inference Server");
        }
        else
        {
            url = AiHelper.RoboflowCloudBaseUrl;
            //Log("HandleAIDetection", "Using Cloud Inference Server");
        }
        string predictions = await AiHelper.InferenceAsync(
            url,
            ImagePath,
            ModelId,
            ModelVersion);

        bool result = HandleResponse.IsPackage(predictions);
        Log("HandleAIDetection", $"The result: {result}");
        return result;
    }
    public static void packageIsDetected(MainFunctions mainFunction, HatchController hatch)
    {
        Log("HelperFunctions", "Package is detected");

        //Take the picture + logging
        /*
        bool success;
        if (mainFunction.handlePicture())
        {
            Log("HelperFunctions", "Picture taken and saved.");
            success = true;
        }

        else
        {
            Log("HelperFunctions", "[ERROR] Failed to take picture.");
            success = false;
        }
        */
        //Update the url
        mainFunction.TriggerApiUpdateUrlAsync();



        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
        else
        {
            Log("HelperFunctions", "Hatch is already open");
        }

        mainFunction.trySentNotification($"http://raspberrypi.local:8081/latest.png");
        Thread.Sleep(5000);
        hatch.MoveHatch(false);
        Log("HelperFunctions", "Hatch closed after 5 seconds");

        Thread.Sleep(5000); // Wait for 5 seconds before the next check
    }
}