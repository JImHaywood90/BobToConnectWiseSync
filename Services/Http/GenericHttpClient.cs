using Abstractions.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

public class GenericHttpClient : IGenericHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenericHttpClient> _logger;

    public GenericHttpClient(HttpClient httpClient, ILogger<GenericHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await SendAsync<TResponse>(request);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data)
    {
        var json = JsonConvert.SerializeObject(data);
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return await SendAsync<TResponse>(request);
    }

    public async Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request)
    {
        _logger.LogInformation("Sending {method} request to {url}", request.Method, request.RequestUri);
        return await _httpClient.SendAsync(request);
    }

    private async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request)
    {
        try
        {
            _logger.LogDebug("Sending request: {method} {url}", request.Method, request.RequestUri);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed HTTP call to {url}", request.RequestUri);
            throw;
        }
    }
}
