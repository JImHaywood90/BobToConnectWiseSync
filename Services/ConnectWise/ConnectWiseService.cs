using Microsoft.Extensions.Logging;
using Abstractions.Services;
using Dto.Bob;
using Dto.ConnectWise;
using Abstractions.Mapping;
using Abstractions;

namespace AzureApiPoc.Services
{
    public class ConnectWiseService : IConnectWiseService
    {
        private readonly IConnectWiseApiClient _apiClient;
        private readonly ILogger<ConnectWiseService> _logger;
        private readonly IModelMapper<HiBobLeaveDetails, ConnectWiseScheduleEntryPayload> _mapper;

        public ConnectWiseService(
            IConnectWiseApiClient apiClient,
            ILogger<ConnectWiseService> logger,
            IModelMapper<HiBobLeaveDetails, ConnectWiseScheduleEntryPayload> mapper)
        {
            _apiClient = apiClient;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ConnectWiseUser?> GetUserByEmailAsync(string email)
        {
            var users = await _apiClient.GetUsersByEmailAsync(email);
            return users.FirstOrDefault();
        }

        public async Task<bool> CreateScheduleEntryAsync(HiBobLeaveDetails details, ConnectWiseUser user)
        {
            var typeId = await _apiClient.GetScheduleTypeIdByNameAsync("Leave");
            if (typeId == null)
            {
                _logger.LogWarning("Schedule type 'Leave' not found. Aborting entry creation.");
                return false;
            }

            var payload = _mapper.Map(details, user.Id, typeId.Value);
            return await _apiClient.PostScheduleEntryAsync(payload);
        }
    }
}
