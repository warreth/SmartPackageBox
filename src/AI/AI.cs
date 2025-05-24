using System.Text.Json;
using System.Text;

using static NonSpecific.Logger;

namespace AI
{
    public static class AiHelper
    {
        private static string ReadApiKey(string filePath)
        {
            string? apiKey = null;
            try
            {
                string json = File.ReadAllText(filePath);
                using JsonDocument doc = JsonDocument.Parse(json);
                apiKey = doc.RootElement.GetProperty("apiKey").GetString();
            }
            catch (Exception e)
            {
                Log("AI.ReadApiKey", $"Error reading API key from file: {e.Message}");
            }
            return apiKey ?? string.Empty;
        }
        private static readonly HttpClient client = new HttpClient();

        public const string RoboflowLocalBaseUrl = "https://inference.wath.dev";
        public const string RoboflowCloudBaseUrl = "https://detect.roboflow.com";

        /// <summary>
        /// Performs inference using a model, image, and specified inference server.
        /// </summary>
        /// <returns>JSON response string on success, or an error on failure.</returns>
        public static async Task<string> InferenceAsync(
            string inferenceServerBaseUrl,
            string imagePath,
            string modelId,
            string modelVersion)
        {
            string apiKey = ReadApiKey("apiKey.json");

            string loggerName = "AI.Inference";

            if (!File.Exists(imagePath))
            {
                Log(loggerName, $"Image file not found: {imagePath}");
                return $"Error: Image file not found at {imagePath}.";
            }

            string fullUrl = $"{inferenceServerBaseUrl.TrimEnd('/')}/{modelId.TrimStart('/')}/{modelVersion}?api_key={apiKey}";
            Log(loggerName, $"Requesting {fullUrl} with image {imagePath}");

            try
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                string base64ImageString = Convert.ToBase64String(imageBytes);
                string imageName = Path.GetFileName(imagePath);

                HttpContent content = new StringContent(base64ImageString, Encoding.UTF8, "application/x-www-form-urlencoded");

                Log(loggerName, $"Posting to: {fullUrl}");

                HttpResponseMessage response = await client.PostAsync(fullUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                content.Dispose();

                if (response.IsSuccessStatusCode)
                {
                    Log(loggerName, $"Success: {response.StatusCode}");
                    return responseBody;
                }
                else
                {
                    Log(loggerName, $"Failed: {response.StatusCode}. Response: {responseBody}");
                    return $"Error: {response.StatusCode} - {responseBody}";
                }
            }

            catch (Exception ex) when (ex is HttpRequestException || ex is IOException)
            {
                Log(loggerName, $"Request or IO error: {ex.Message}");
                return $"Error: {ex.GetType().Name} - {ex.Message}.";
            }
            catch (Exception ex)
            {
                Log(loggerName, $"Unexpected error: {ex.Message}");
                return $"Error: Unexpected - {ex.Message}.";
            }
        }
    }
    public class HandleResponse
    {
        public static bool IsPackage(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Log(jsonString, "Warning: Empty or null JSON string provided.");
                return false;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(jsonString); // properly dispose of the document
                JsonElement root = document.RootElement;

                // Check the 'top' field
                if (root.TryGetProperty("top", out JsonElement topElement) && topElement.ValueKind == JsonValueKind.String)
                {
                    if ("package".Equals(topElement.GetString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Log(jsonString, "Detected package based on 'top' field.");
                        return true; // It's a package based on the 'top' field
                    }
                }

                // Check the 'predictions' array
                if (root.TryGetProperty("predictions", out JsonElement predictionsElement) && predictionsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement prediction in predictionsElement.EnumerateArray())
                    {
                        if (prediction.TryGetProperty("class", out JsonElement classElement) &&
                            classElement.ValueKind == JsonValueKind.String)
                        {
                            if ("package".Equals(classElement.GetString(), StringComparison.OrdinalIgnoreCase))
                            {
                                Log(jsonString, "Detected package based on prediction class.");
                                return true; // It's a package based on a prediction's class
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Error parsing JSON
                Log(jsonString, "Error parsing JSON response.");
                return false;
            }

            // If no conditions matched, it's not a package
            return false;
        }
    }
}

