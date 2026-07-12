namespace Munters.Giphy.Api.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public int SearchExpirationMinutes { get; init; } = 60;

    public int TrendingExpirationMinutes { get; init; } = 10;
}