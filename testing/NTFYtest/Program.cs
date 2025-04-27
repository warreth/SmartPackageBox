using System;
using System.Threading.Tasks;
using Ntfy;

class Program
{
    static async Task Main(string[] args)
    {
        // Test the Notifications.sendNotification method with a timestamped message
        DateTime now = DateTime.Now;
        string msg = $"test, {now.ToString("yyyy-MM-dd HH:mm:ss")}";
        await Notifications.sendNotification(msg);
    }
}