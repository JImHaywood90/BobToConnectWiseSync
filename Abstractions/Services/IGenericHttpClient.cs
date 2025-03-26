using System.Threading.Tasks;

namespace Abstractions.Services
{
    public interface IGenericHttpClient
    {
        Task<TResponse> GetAsync<TResponse>(string uri);
        Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest data);
        Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request); // For edge cases
    }

}
