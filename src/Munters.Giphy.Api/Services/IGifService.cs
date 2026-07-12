using Munters.Giphy.Api.Models;

namespace Munters.Giphy.Api.Services;

public interface IGifService
{
    Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken);
}