using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Services;

public class ApiService
{
    private readonly HttpClient _client;

    public ApiService()
    {
        // WARNING: This bypasses SSL certificate validation and is INSECURE. Use only for development with self-signed certificates.
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _client = new HttpClient(handler);
    }
    public async Task<string> ContactUrlAsync(string pUrl, string endpoint)
    {
        if (string.IsNullOrWhiteSpace(pUrl))
            throw new System.ArgumentException("API URL cannot be null or empty.", nameof(pUrl));
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new System.ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));

        string fullUrl = $"{pUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

        try
        {
            HttpResponseMessage response = await _client.GetAsync(fullUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonContent))
                {
                    return "String is empty or null.";
                }

                // Deserialize the JSON string + check for null result
                string? result = System.Text.Json.JsonSerializer.Deserialize<string>(jsonContent);
                if (result == null)
                {
                    return "Error: Deserialized string is null.";
                }
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
        catch (System.Text.Json.JsonException e)
        {
            return $"JsonError: {e.Message}";
        }
        catch (HttpRequestException e)
        {
            return $"HttpError: {e.Message}";
        }
        catch (Exception e)
        {
            return $"ExcepError: {e.Message}";
        }
    }

}