using Microsoft.Extensions.Caching.Memory;

namespace Munters.Giphy.Api.Caching;

public sealed class MemoryRequestCache : IRequestCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryRequestCache> _logger;

    public MemoryRequestCache(
        IMemoryCache memoryCache,
        ILogger<MemoryRequestCache> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan expiration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue) &&
            cachedValue is not null)
        {
            _logger.LogInformation(
                "Cache hit for key {CacheKey}",
                key);

            return cachedValue;
        }

        _logger.LogInformation(
            "Cache miss for key {CacheKey}",
            key);

        var value = await factory(cancellationToken);

        _memoryCache.Set(
            key,
            value,
            expiration);

        return value;
    }
}