using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dto.ConnectWise;
using Abstractions;

namespace AzureApiPoc.Services.ConnectWise
{
    public class ConnectWiseApiClient : IConnectWiseApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConnectWiseApiClient> _logger;

        public ConnectWiseApiClient(HttpClient httpClient, ILogger<ConnectWiseApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ConnectWiseUser>> GetUsersByEmailAsync(string email)
        {
            var url = $"/system/members?conditions=Email='{email}'";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ConnectWiseUser>>(json) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by email from ConnectWise.");
                return new List<ConnectWiseUser>();
            }
        }

        public async Task<int?> GetScheduleTypeIdByNameAsync(string name)
        {
            var url = $"/schedule/types/info?conditions=name=\"{name}\"";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var types = JsonConvert.DeserializeObject<List<ScheduleTypeInfo>>(json);
                return types?.FirstOrDefault()?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule type ID from ConnectWise.");
                return null;
            }
        }

        public async Task<bool> PostScheduleEntryAsync(ConnectWiseScheduleEntryPayload entry)
        {
            try
            {
                var json = JsonConvert.SerializeObject(entry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/schedule/entries", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create schedule entry. Status: {StatusCode}, Response: {Response}", response.StatusCode, error);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting schedule entry to ConnectWise.");
                return false;
            }
        }
    }
}
