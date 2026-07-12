using Munters.Giphy.Api.Clients;
using Munters.Giphy.Api.Models;

namespace Munters.Giphy.Api.Services;

public sealed class GifService : IGifService
{
    private readonly IGiphyClient _giphyClient;

    public GifService(IGiphyClient giphyClient)
    {
        _giphyClient = giphyClient;
    }

    public Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken)
    {
        return _giphyClient.GetTrendingAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var normalizedSearchTerm = searchTerm.Trim();

        return _giphyClient.SearchAsync(
            normalizedSearchTerm,
            cancellationToken);
    }
}