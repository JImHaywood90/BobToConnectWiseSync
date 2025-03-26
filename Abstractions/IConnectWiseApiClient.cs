using Dto.ConnectWise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IConnectWiseApiClient
    {
        Task<List<ConnectWiseUser>> GetUsersByEmailAsync(string email);
        Task<int?> GetScheduleTypeIdByNameAsync(string name);
        Task<bool> PostScheduleEntryAsync(ConnectWiseScheduleEntryPayload entry);
    }

}
