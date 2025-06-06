using AI;
using Functions;
using MotorController;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using Api;
using Emgu.CV;

// Main entry point
public class Program
{
    // Image server and API URLs
    public static string imageServerUrl = "https://localhost:8081";
    public static string apiUrl = "https://localhost:8080";
    public static void Main(string[] args)
    {
        // Create shared hatch controller
        HatchController sharedHatch = new();
        // Set API base URL
        Helper.BaseUrl = apiUrl;

        // Get singleton instance for main functions
        MainFunctions mainFunction = MainFunctions.Instance;

        // Start API and image server in background threads
        Task.Run(() => ApiHost.Start(sharedHatch, "8080"));
        Task.Run(() => ImageServer.Start("8081"));

        string imagePath = "./wwwroot/latest.png";
        // Main loop: check for package detection
        while (true)
        {
            if (mainFunction.enableDetection)
            {
                // Take picture
                mainFunction.handlePicture(imagePath);

                // Run AI detection
                if (HandleAIDetection(imagePath).GetAwaiter().GetResult())
                {
                    Log("Program", "Package is detected");
                    // Handle detected package
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
            Thread.Sleep(5000); // Wait before next check
        }
    }
    // Run AI detection on image
    public static async Task<bool> HandleAIDetection(string imagePath, bool isLocal = true)
    {
        string url = string.Empty;
        string ImagePath = imagePath;
        string ModelId = "packagedetection-xyah9";
        string ModelVersion = "3";

        if (isLocal)
        {
            url = AiHelper.RoboflowLocalBaseUrl;
        }
        else
        {
            url = AiHelper.RoboflowCloudBaseUrl;
        }
        // Get predictions from AI
        string predictions = await AiHelper.InferenceAsync(
            url,
            ImagePath,
            ModelId,
            ModelVersion);

        // Check if package is detected
        bool result = HandleResponse.IsPackage(predictions);
        Log("HandleAIDetection", $"The result: {result}");
        return result;
    }
    // Handle actions when package is detected
    public static void packageIsDetected(MainFunctions mainFunction, HatchController hatch)
    {
        Log("HelperFunctions", "Package is detected");

        // Update image URL in API
        mainFunction.TriggerApiUpdateUrlAsync();

        // Open hatch if closed
        if (!hatch.hatchProperties.isOpen)
        {
            hatch.MoveHatch(true);
        }
        else
        {
            Log("HelperFunctions", "Hatch is already open");
        }

        // Send notification
        mainFunction.trySentNotification($"http://raspberrypi.local:8081/latest.png");
        Thread.Sleep(5000); // Wait before closing hatch
        hatch.MoveHatch(false);
        Log("HelperFunctions", "Hatch closed after 5 seconds");

        Thread.Sleep(5000); // Wait before next check
    }
}