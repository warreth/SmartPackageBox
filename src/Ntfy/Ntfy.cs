namespace Ntfy;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

public static class Notifications
{
    // Reads ntfy config from a JSON file and returns token, domain, and topic
    private static (string token, string domain, string topic) ReadNtfyConfig(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            string token = doc.RootElement.GetProperty("token").GetString();
            string domain = doc.RootElement.GetProperty("domain").GetString();
            string topic = doc.RootElement.GetProperty("topic").GetString();
            return (token, domain, topic);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to read config: {ex.Message}");
            return (null, null, null);
        }
    }

    // Sends a notification to the ntfy server
    public static async Task sendNotification(string message, string actions = null, byte[] pImage = null)
    {
        var (token, domain, topic) = ReadNtfyConfig("token.json");
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(topic))
        {
            Console.WriteLine("[ERROR] Config missing or invalid");
            return;
        }
        string url = $"https://{domain}/{topic}";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Add("Authorization", $"Bearer {token}");
        if (!string.IsNullOrWhiteSpace(actions)) request.Headers.Add("Actions", actions);
        request.Content = new System.Net.Http.StringContent(message);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        Console.WriteLine($"[DEBUG] POST {url}");
        Console.WriteLine($"[DEBUG] Authorization: Bearer {token}");
        if (!string.IsNullOrWhiteSpace(actions)) Console.WriteLine($"[DEBUG] Actions: {actions}");
        Console.WriteLine($"[DEBUG] Content-Type: application/x-www-form-urlencoded");
        Console.WriteLine($"[DEBUG] Body: {message}");

        try
        {
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ntfy] Response: {responseBody}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ERROR] HTTP Request Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Exception: {ex.Message}");
        }
    }
}