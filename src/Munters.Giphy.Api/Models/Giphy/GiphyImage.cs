using System.Text.Json.Serialization;

namespace Munters.Giphy.Api.Models.Giphy;

public sealed class GiphyImage
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}