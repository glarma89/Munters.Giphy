using Munters.Giphy.Api.Models;

namespace Munters.Giphy.Api.Services;

public sealed class GifService : IGifService
{
    public Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<GifDto> gifs =
        [
            new GifDto("https://example.com/trending-gif-1.gif"),
            new GifDto("https://example.com/trending-gif-2.gif")
        ];

        return Task.FromResult(gifs);
    }

    public Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedSearchTerm = searchTerm.Trim();

        IReadOnlyCollection<GifDto> gifs =
        [
            new GifDto(
                $"https://example.com/{Uri.EscapeDataString(normalizedSearchTerm)}-gif-1.gif"),
            new GifDto(
                $"https://example.com/{Uri.EscapeDataString(normalizedSearchTerm)}-gif-2.gif")
        ];

        return Task.FromResult(gifs);
    }
}