using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

public class POGApiClient
{
    private readonly string _pogEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private readonly HttpClient _httpClient;

    private readonly ILogger _logger;

    public POGApiClient(ILogger logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();

    }


    public POGApiClient(ILogger logger, string pogEndpoint, string clientId, string clientSecret)
    {
        _logger = logger;
        _pogEndpoint = pogEndpoint;
        _clientId = clientId;
        _clientSecret = clientSecret;

        _httpClient = new HttpClient();
    }

    public ILogger Logger { get; }

    public async Task<string> GetAccessTokenAsync(string pogEndpoint, string clientId, string clientSecret)
    {
        try
        {
            byte[] basicAuth = System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret);
            string basicAuthEncoded = System.Convert.ToBase64String(basicAuth);
            var httpTokenClient = new HttpClient()
            {
                BaseAddress = new Uri(pogEndpoint)
            };
            httpTokenClient.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuthEncoded);

            // Add the necessary data for the request
            HttpContent data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            });

            HttpResponseMessage tokenResponse = await httpTokenClient.PostAsync("/OAuth/Token", data);
            tokenResponse.EnsureSuccessStatusCode();

            string jsonString = await tokenResponse.Content.ReadAsStringAsync();
            string accessToken = (string)JObject.Parse(jsonString)["access_token"];
            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access token");
            throw; // Rethrow the exception for handling in the caller
        }
    }

    public async Task PostTimeTrackingEntriesAsync(IEnumerable<object> requestObjects, string accessToken)
    {
        try
        {
            if (_httpClient != null && _httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            foreach (var item in requestObjects)
            {
                HttpContent jsonContent = JsonContent.Create(item);
                HttpResponseMessage postResponse = await _httpClient.PostAsync("https://api-demo.poweroffice.net/TimeTracking/TimeTrackingEntry", jsonContent);
                postResponse.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting time tracking entries");
            throw; // Rethrow the exception for handling in the caller
        }
    }
}
