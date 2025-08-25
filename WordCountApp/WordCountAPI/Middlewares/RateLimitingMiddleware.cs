namespace WordCountAPI.Middlewares;

public class RateLimitingMiddleware : IMiddleware
{
    private static readonly Dictionary<string, (int Count, DateTime Reset)> _clients = new();
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(int maxRequests = 10, int seconds = 60)
    {
        _maxRequests = maxRequests;
        _timeWindow = TimeSpan.FromSeconds(seconds);
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        lock (_clients)
        {
            if (!_clients.TryGetValue(clientId, out var entry) || entry.Reset <= DateTime.UtcNow)
            {
                _clients[clientId] = (1, DateTime.UtcNow.Add(_timeWindow));
            }
            else if (entry.Count >= _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.RetryAfter = ((int)(entry.Reset - DateTime.UtcNow).TotalSeconds).ToString();
                return;
            }
            else
            {
                _clients[clientId] = (entry.Count + 1, entry.Reset);
            }
        }
        
        await next(context);
    }
}