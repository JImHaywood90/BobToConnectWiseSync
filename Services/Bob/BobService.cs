using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dto.Bob;
using Abstractions.Services;

namespace Services.Bob
{

    public class BobService : IBobService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BobService> _logger;

        public BobService(HttpClient httpClient, ILogger<BobService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HiBobLeaveDetails?> GetLeaveDetailsAsync(string getApiUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(getApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var details = JsonConvert.DeserializeObject<HiBobLeaveDetails>(content);
                    return details;
                }
                _logger.LogError("Bob API returned status code: {statusCode}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when calling Bob API");
            }
            return null;
        }
    }
}
