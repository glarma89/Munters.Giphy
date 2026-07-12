using Microsoft.Extensions.Options;
using Munters.Giphy.Api.Caching;
using Munters.Giphy.Api.Clients;
using Munters.Giphy.Api.Models;
using Munters.Giphy.Api.Options;

namespace Munters.Giphy.Api.Services;

public sealed class GifService : IGifService
{
    private const string TrendingCacheKey = "giphy:trending";

    private readonly IGiphyClient _giphyClient;
    private readonly IRequestCache _cache;
    private readonly CacheOptions _cacheOptions;

    public GifService(
        IGiphyClient giphyClient,
        IRequestCache cache,
        IOptions<CacheOptions> cacheOptions)
    {
        _giphyClient = giphyClient;
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
    }

    public Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken)
    {
        var expiration = TimeSpan.FromMinutes(
            _cacheOptions.TrendingExpirationMinutes);

        return _cache.GetOrCreateAsync(
            TrendingCacheKey,
            expiration,
            token => _giphyClient.GetTrendingAsync(token),
            cancellationToken);
    }

    public Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var normalizedSearchTerm = NormalizeSearchTerm(searchTerm);
        var cacheKey = $"giphy:search:{normalizedSearchTerm}";

        var expiration = TimeSpan.FromMinutes(
            _cacheOptions.SearchExpirationMinutes);

        return _cache.GetOrCreateAsync(
            cacheKey,
            expiration,
            token => _giphyClient.SearchAsync(
                normalizedSearchTerm,
                token),
            cancellationToken);
    }

    private static string NormalizeSearchTerm(string searchTerm)
    {
        return searchTerm
            .Trim()
            .ToLowerInvariant();
    }
}