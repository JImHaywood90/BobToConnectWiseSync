using System.Threading.Tasks;
using Dto.Bob;
using Dto.ConnectWise;

namespace Abstractions.Services
{
    public interface IConnectWiseService
    {
        Task<ConnectWiseUser?> GetUserByEmailAsync(string email);
        Task<bool> CreateScheduleEntryAsync(HiBobLeaveDetails details, ConnectWiseUser user);
    }
}
