namespace Ntfy;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

// Notification sending via ntfy
public static class Notifications
{
    // Reads ntfy config from a JSON file and returns token, domain, and topic
    private static (string token, string domain, string topic) ReadNtfyConfig(string fileUrl)
    {
        try
        {
            // Read config file
            string json = File.ReadAllText(fileUrl);
            using var doc = JsonDocument.Parse(json);
            string token = doc.RootElement.GetProperty("token").GetString();
            string domain = doc.RootElement.GetProperty("domain").GetString();
            string topic = doc.RootElement.GetProperty("topic").GetString();
            return (token, domain, topic);
        }
        catch (Exception ex)
        {
            Log("Ntfy", $"Failed to read config: {ex.Message}");
            return (null, null, null);
        }
    }

    // Sends a notification to the ntfy server
    public static async Task sendNotification(string message, string title = null, string actions = null, string pImageUrl = null)
    {
        // Read config and validate
        var (token, domain, topic) = ReadNtfyConfig("token.json");
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(topic))
        {
            Log("Ntfy", "Config missing or invalid");
            return;
        }
        string url = $"https://{domain}/{topic}";

        using HttpClient client = new HttpClient(); //This autodisposes of the Object after use
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

        // Add Authorization header
        request.Headers.Add("Authorization", $"Bearer {token}");

        // Add Actions header if provided
        if (!string.IsNullOrWhiteSpace(actions))
        {
            string cleanedActions = actions.TrimEnd(';');
            request.Headers.Add("Actions", cleanedActions);
        }
        // Add Attach header if image Url is provided and file exists
        if (!string.IsNullOrWhiteSpace(pImageUrl))
        {
            if (Uri.IsWellFormedUriString(pImageUrl, UriKind.Absolute))
            {
                request.Headers.Add("Attach", pImageUrl);
                Log("Ntfy", $"Attach: {pImageUrl}");
            }
            else
            {
                Log("Ntfy", $"Attach must be a valid URL: {pImageUrl}");
            }
        }
        // Add Title header if provided
        if (!string.IsNullOrWhiteSpace(title))
        {
            request.Headers.Add("X-Title", title);
            Log("Ntfy", $"Title: {title}");
        }

        // Set message as plain text content
        request.Content = new System.Net.Http.StringContent(message);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        // Debug output
        Log("Ntfy", $"POST {url}");
        Log("Ntfy", $"Authorization: Bearer {token}");
        if (!string.IsNullOrWhiteSpace(actions)) Log("Ntfy", $"Actions: {actions}");
        Log("Ntfy", "Content-Type: text/plain");
        Log("Ntfy", $"Body: {message}");

        try
        {
            // Send the request and check for errors
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Log("Ntfy", $"Response: {responseBody}");
        }
        catch (HttpRequestException ex)
        {
            Log("Ntfy", $"HTTP Request Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log("Ntfy", $"Exception: {ex.Message}");
        }
    }
}