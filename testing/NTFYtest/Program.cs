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

        await Notifications.sendNotification(msg, actions);
    }
}