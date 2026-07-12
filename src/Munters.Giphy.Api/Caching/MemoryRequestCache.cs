using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Munters.Giphy.Api.Caching;

public sealed class MemoryRequestCache : IRequestCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryRequestCache> _logger;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks =
        new();

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
        if (TryGetCachedValue(key, out T? cachedValue))
        {
            _logger.LogInformation(
                "Cache hit for key {CacheKey}",
                key);

            return cachedValue!;
        }

        _logger.LogInformation(
            "Cache miss for key {CacheKey}",
            key);

        var keyLock = _locks.GetOrAdd(
            key,
            static _ => new SemaphoreSlim(1, 1));

        await keyLock.WaitAsync(cancellationToken);

        try
        {
            if (TryGetCachedValue(key, out cachedValue))
            {
                _logger.LogInformation(
                    "Cache filled while waiting for key {CacheKey}",
                    key);

                return cachedValue!;
            }

            var value = await factory(cancellationToken);

            _memoryCache.Set(
                key,
                value,
                expiration);

            return value;
        }
        finally
        {
            keyLock.Release();
        }
    }

    private bool TryGetCachedValue<T>(
        string key,
        out T? value)
    {
        return _memoryCache.TryGetValue(key, out value) &&
               value is not null;
    }
}