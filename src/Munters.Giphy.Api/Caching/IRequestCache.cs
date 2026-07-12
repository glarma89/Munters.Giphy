namespace Munters.Giphy.Api.Caching;

public interface IRequestCache
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan expiration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken);
}