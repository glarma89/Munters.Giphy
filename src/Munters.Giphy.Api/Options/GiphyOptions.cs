namespace Munters.Giphy.Api.Options;

public sealed class GiphyOptions
{
    public const string SectionName = "Giphy";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    public int SearchLimit { get; init; } = 25;

    public int TrendingLimit { get; init; } = 25;

    public string Rating { get; init; } = "g";
}