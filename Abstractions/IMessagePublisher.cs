namespace Abstractions
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T model);

        Task PublishAsync(string message);
    }
}
