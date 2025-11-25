using System.Collections.Concurrent;

namespace Spotify.Core.Services;

public interface IRateLimitService
{
    bool IsAllowed(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow);
    (bool allowed, int remaining, TimeSpan resetAfter) GetRateLimitInfo(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow);
}

public class RateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, ClientRateLimit> _rateLimits = new();

    public bool IsAllowed(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow)
    {
        var key = $"{endpoint}:{identifier}";
        var now = DateTime.UtcNow;

        if (!_rateLimits.TryGetValue(key, out var rateLimit))
        {
            rateLimit = new ClientRateLimit
            {
                Endpoint = endpoint,
                Identifier = identifier,
                MaxRequests = maxRequests,
                TimeWindow = timeWindow,
                Requests = new List<DateTime> { now }
            };
            _rateLimits[key] = rateLimit;
            return true;
        }

        // Limpiar requests antiguos
        rateLimit.Requests.RemoveAll(r => r < now - timeWindow);

        if (rateLimit.Requests.Count >= maxRequests)
        {
            return false;
        }

        rateLimit.Requests.Add(now);
        return true;
    }

    public (bool allowed, int remaining, TimeSpan resetAfter) GetRateLimitInfo(
        string endpoint, string identifier, int maxRequests, TimeSpan timeWindow)
    {
        var key = $"{endpoint}:{identifier}";
        var now = DateTime.UtcNow;

        if (!_rateLimits.TryGetValue(key, out var rateLimit))
        {
            return (true, maxRequests, timeWindow);
        }

        rateLimit.Requests.RemoveAll(r => r < now - timeWindow);
        var remaining = Math.Max(0, maxRequests - rateLimit.Requests.Count);
        var oldestRequest = rateLimit.Requests.Min();
        var resetAfter = timeWindow - (now - oldestRequest);

        return (rateLimit.Requests.Count < maxRequests, remaining, resetAfter);
    }

    // Limpieza periódica de rate limits expirados
    public void CleanupExpiredRateLimits()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _rateLimits
            .Where(kvp => kvp.Value.Requests.All(r => r < now - kvp.Value.TimeWindow))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _rateLimits.TryRemove(key, out _);
        }
    }

    private class ClientRateLimit
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public int MaxRequests { get; set; }
        public TimeSpan TimeWindow { get; set; }
        public List<DateTime> Requests { get; set; } = new List<DateTime>();
    }
}