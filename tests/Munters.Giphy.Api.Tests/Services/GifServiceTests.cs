using Microsoft.Extensions.Options;
using Moq;
using Munters.Giphy.Api.Caching;
using Munters.Giphy.Api.Clients;
using Munters.Giphy.Api.Models;
using Munters.Giphy.Api.Options;
using Munters.Giphy.Api.Services;

namespace Munters.Giphy.Api.Tests.Services;

public sealed class GifServiceTests
{
    [Fact]
    public async Task SearchAsync_WhenResultIsCached_ReturnsCachedResultWithoutCallingClient()
    {
        // Arrange
        const int searchExpirationMinutes = 5;

        IReadOnlyCollection<GifDto> cachedGifs =
        [
            new GifDto("https://example.com/cached.gif")
        ];

        var giphyClientMock = new Mock<IGiphyClient>();
        var cacheMock = new Mock<IRequestCache>();

        cacheMock
            .Setup(cache => cache.GetOrCreateAsync(
                "giphy:search:cat",
                TimeSpan.FromMinutes(searchExpirationMinutes),
                It.IsAny<Func<
                    CancellationToken,
                    Task<IReadOnlyCollection<GifDto>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedGifs);

        var cacheOptions = Microsoft.Extensions.Options.Options.Create(
            new CacheOptions
            {
                SearchExpirationMinutes = searchExpirationMinutes,
                TrendingExpirationMinutes = 10
            });

        var service = new GifService(
            giphyClientMock.Object,
            cacheMock.Object,
            cacheOptions);

        // Act
        var result = await service.SearchAsync(
            "cat",
            CancellationToken.None);

        // Assert
        Assert.Same(cachedGifs, result);

        giphyClientMock.Verify(
            client => client.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        cacheMock.Verify(
            cache => cache.GetOrCreateAsync(
                "giphy:search:cat",
                TimeSpan.FromMinutes(searchExpirationMinutes),
                It.IsAny<Func<
                    CancellationToken,
                    Task<IReadOnlyCollection<GifDto>>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenResultIsNotCached_CallsClientOnceAndReturnsClientResult()
    {
        // Arrange
        const int searchExpirationMinutes = 5;

        IReadOnlyCollection<GifDto> clientGifs =
        [
            new GifDto("https://example.com/client.gif")
        ];

        var giphyClientMock = new Mock<IGiphyClient>();
        var cacheMock = new Mock<IRequestCache>();

        giphyClientMock
            .Setup(client => client.SearchAsync(
                "cat",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientGifs);

        cacheMock
            .Setup(cache => cache.GetOrCreateAsync(
                "giphy:search:cat",
                TimeSpan.FromMinutes(searchExpirationMinutes),
                It.IsAny<Func<
                    CancellationToken,
                    Task<IReadOnlyCollection<GifDto>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(
                (
                    string _,
                    TimeSpan _,
                    Func<CancellationToken, Task<IReadOnlyCollection<GifDto>>> factory,
                    CancellationToken cancellationToken) =>
                    factory(cancellationToken));

        var cacheOptions = Microsoft.Extensions.Options.Options.Create(
            new CacheOptions
            {
                SearchExpirationMinutes = searchExpirationMinutes,
                TrendingExpirationMinutes = 10
            });

        var service = new GifService(
            giphyClientMock.Object,
            cacheMock.Object,
            cacheOptions);

        // Act
        var result = await service.SearchAsync(
            "cat",
            CancellationToken.None);

        // Assert
        Assert.Same(clientGifs, result);

        giphyClientMock.Verify(
            client => client.SearchAsync(
                "cat",
                It.IsAny<CancellationToken>()),
            Times.Once);

        cacheMock.Verify(
            cache => cache.GetOrCreateAsync(
                "giphy:search:cat",
                TimeSpan.FromMinutes(searchExpirationMinutes),
                It.IsAny<Func<
                    CancellationToken,
                    Task<IReadOnlyCollection<GifDto>>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}