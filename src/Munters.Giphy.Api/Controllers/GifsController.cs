using Microsoft.AspNetCore.Mvc;
using Munters.Giphy.Api.Models;
using Munters.Giphy.Api.Services;

namespace Munters.Giphy.Api.Controllers;

[ApiController]
[Route("api/gifs")]
public sealed class GifsController : ControllerBase
{
    private readonly IGifService _gifService;

    public GifsController(IGifService gifService)
    {
        _gifService = gifService;
    }

    [HttpGet("trending")]
    [ProducesResponseType<IReadOnlyCollection<GifDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<GifDto>>> GetTrending(
        CancellationToken cancellationToken)
    {
        var gifs = await _gifService.GetTrendingAsync(cancellationToken);

        return Ok(gifs);
    }

    [HttpGet("search")]
    [ProducesResponseType<IReadOnlyCollection<GifDto>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<GifDto>>> Search(
        [FromQuery] string? term,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(new
            {
                Error = "Search term is required."
            });
        }

        var gifs = await _gifService.SearchAsync(
            term,
            cancellationToken);

        return Ok(gifs);
    }
}