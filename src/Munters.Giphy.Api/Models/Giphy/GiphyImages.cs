using System.Text.Json.Serialization;

namespace Munters.Giphy.Api.Models.Giphy;

public sealed class GiphyImages
{
    [JsonPropertyName("original")]
    public GiphyImage Original { get; init; } = new();
}