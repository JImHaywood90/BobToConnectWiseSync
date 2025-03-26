using System.Threading.Tasks;
using Dto.Bob;

namespace Abstractions.Services
{
    public interface IBobService
    {
        Task<HiBobLeaveDetails?> GetLeaveDetailsAsync(string getApiUrl);
    }
}
