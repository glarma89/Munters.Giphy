using System.Text.Json.Serialization;

namespace Munters.Giphy.Api.Models.Giphy;

public sealed class GiphyApiResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyCollection<GiphyGifData> Data { get; init; } =
        Array.Empty<GiphyGifData>();
}