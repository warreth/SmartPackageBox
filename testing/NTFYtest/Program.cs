using System;
using System.Threading.Tasks;
using Ntfy;

class Program
{
    static async Task Main(string[] args)
    {
        // Test the Notifications.sendNotification method with a timestamped message and actions
        DateTime now = DateTime.Now;
        string msg = $"test, {now.ToString("dd-MM-yy HH:mm:ss")}";
        // Define the actions header
        string actions = "view, Open App, smartpackagebox://open, clear=true;";

        string imageUrl = "https://yavuzceliker.github.io/sample-images/image-9.jpg";
        await Notifications.sendNotification(msg, "Package Detected!", actions, imageUrl);
    }/*
    public static void Main(string[] args)
    {
        // Test the Notifications.sendNotification method with a timestamped message and actions
        DateTime now = DateTime.Now;
        string msg = $"test, {now.ToString("dd-MM-yy HH:mm:ss")}";
        // Define the actions header
        string actions = "view, Open App, smartpackagebox://open, clear=true;";

        string imageUrl = "https://picsum.photos/200/300";

        // Run the async method synchronously and handle exceptions
        try
        {
            // Wait for the async method to complete
            Notifications.sendNotification(msg, "Package Detected!", actions, imageUrl)
                .GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // Print any errors to the console
            Console.WriteLine($"Error sending notification: {ex.Message}");
        }
    }*/
}