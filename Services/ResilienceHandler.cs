using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class ResilienceHandler : DelegatingHandler
{
    private readonly int _maxRetries = 3;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    private readonly Random _jitter = new Random();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int retryCount = 0;

        while (true)
        {
            using var cts = new CancellationTokenSource(_timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            try
            {
                return await base.SendAsync(request, linkedCts.Token);
            }
            catch (HttpRequestException) when (retryCount < _maxRetries)
            {
                retryCount++;
                await Task.Delay(GetDelay(retryCount), cancellationToken);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && retryCount < _maxRetries)
            {
                // Likely a timeout
                retryCount++;
                await Task.Delay(GetDelay(retryCount), cancellationToken);
            }
        }
    }

    private TimeSpan GetDelay(int retryCount)
    {
        var backoff = Math.Pow(2, retryCount); // 2, 4, 8...
        var jitter = _jitter.Next(0, 1000);    // up to 1s random
        return TimeSpan.FromSeconds(backoff) + TimeSpan.FromMilliseconds(jitter);
    }
}
