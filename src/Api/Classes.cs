using System.Security.Cryptography;
namespace Api;
public class ApiInfo
{
    public string mNewestUrl;
    protected string baseUrl = "https://example.com/images/latest.jpg";
    protected string mQueryParameter;
}

public class Helper : ApiInfo
{
    // Shared instance for global use
    public static readonly Helper Instance = new Helper();

    public Helper() { } // Public constructor allows creating new instances if needed

    public void UpdateUrl() // Update the URL when a new photo is uploaded
    {
        try
        {
            // Calculate and update the query parameter
            UpdateQueryParameter(CalculateQueryParameter());
            // Update the newest URL
            mNewestUrl = baseUrl + mQueryParameter;
        }
        catch (Exception ex)
        {
            // Error checking: log or handle error as needed
            throw new Exception($"Failed to update URL: {ex.Message}");
        }
    }

    private void UpdateQueryParameter(string pNewQueryParameter)
    {
        // Error checking: ensure parameter is not null
        if (pNewQueryParameter == null)
        {
            throw new ArgumentNullException(nameof(pNewQueryParameter), "Query parameter cannot be null");
        }
        mQueryParameter = pNewQueryParameter;
    }

    private string CalculateQueryParameter()
    {
        // Generate a timestamp-based query parameter
        var tTimestamp = DateTime.Now.Ticks;
        return $"?t={tTimestamp}";
    }
}