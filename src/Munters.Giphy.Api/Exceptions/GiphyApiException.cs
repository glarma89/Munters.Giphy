using System.Net;

namespace Munters.Giphy.Api.Exceptions;

public sealed class GiphyApiException : Exception
{
    public GiphyApiException(
        HttpStatusCode upstreamStatusCode,
        string message)
        : base(message)
    {
        UpstreamStatusCode = upstreamStatusCode;
    }

    public HttpStatusCode UpstreamStatusCode { get; }
}