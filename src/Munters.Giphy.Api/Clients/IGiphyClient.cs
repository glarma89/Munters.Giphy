using Munters.Giphy.Api.Models;

namespace Munters.Giphy.Api.Clients;

public interface IGiphyClient
{
    Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken);
}