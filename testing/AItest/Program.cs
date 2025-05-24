using System;
using System.IO;
using System.Threading.Tasks;
using AI;

class Program
{

    static async Task Main(string[] args)
    {
        string url = string.Empty;
        string ImagePath = "./Package.jpg";
        string ModelId = "packagedetection-xyah9";
        string ModelVersion = "3";

        System.Console.WriteLine("Test Local(1) or cloud(2)?");
        string choice = Console.ReadLine();
        if (choice == "1")
        {
            url = AiHelper.RoboflowLocalBaseUrl;
            Console.WriteLine("Testing Local Inference Server");
        }
        else if (choice == "2")
        {
            url = AiHelper.RoboflowCloudBaseUrl;
            System.Console.WriteLine("Testing Cloud Inference Server");
        }
        string predictions = await AiHelper.InferenceAsync(
            url,
            ImagePath,
            ModelId,
            ModelVersion);

        Console.WriteLine($"The result: {HandleResponse.IsPackage(predictions)}");
    }
}