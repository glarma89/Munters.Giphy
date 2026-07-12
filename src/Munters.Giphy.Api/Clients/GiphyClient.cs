using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Munters.Giphy.Api.Models;
using Munters.Giphy.Api.Models.Giphy;
using Munters.Giphy.Api.Options;

namespace Munters.Giphy.Api.Clients;

public sealed class GiphyClient : IGiphyClient
{
    private readonly HttpClient _httpClient;
    private readonly GiphyOptions _options;

    public GiphyClient(
        HttpClient httpClient,
        IOptions<GiphyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public Task<IReadOnlyCollection<GifDto>> GetTrendingAsync(
        CancellationToken cancellationToken)
    {
        var requestUri =
            $"v1/gifs/trending" +
            $"?api_key={Uri.EscapeDataString(_options.ApiKey)}" +
            $"&limit={_options.TrendingLimit}" +
            $"&rating={Uri.EscapeDataString(_options.Rating)}" +
            "&fields=images";

        return GetGifUrlsAsync(requestUri, cancellationToken);
    }

    public Task<IReadOnlyCollection<GifDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var requestUri =
            $"v1/gifs/search" +
            $"?api_key={Uri.EscapeDataString(_options.ApiKey)}" +
            $"&q={Uri.EscapeDataString(searchTerm)}" +
            $"&limit={_options.SearchLimit}" +
            $"&rating={Uri.EscapeDataString(_options.Rating)}" +
            "&fields=images";

        return GetGifUrlsAsync(requestUri, cancellationToken);
    }

    private async Task<IReadOnlyCollection<GifDto>> GetGifUrlsAsync(
        string requestUri,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var giphyResponse =
            await response.Content.ReadFromJsonAsync<GiphyApiResponse>(
                cancellationToken: cancellationToken);

        if (giphyResponse is null)
        {
            return Array.Empty<GifDto>();
        }

        return giphyResponse.Data
            .Select(gif => gif.Images.Original.Url)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => new GifDto(url))
            .ToArray();
    }
}