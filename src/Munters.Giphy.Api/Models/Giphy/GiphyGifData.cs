using System.Text.Json.Serialization;

namespace Munters.Giphy.Api.Models.Giphy;

public sealed class GiphyGifData
{
    [JsonPropertyName("images")]
    public GiphyImages Images { get; init; } = new();
}