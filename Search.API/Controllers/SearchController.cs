using Microsoft.AspNetCore.Mvc;
using Search.API.Services;

namespace Search.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ElasticsearchService _elasticsearchService;

    public SearchController(ElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
    [FromQuery] string query,
    [FromQuery] int size = 400)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required");

        var results = await _elasticsearchService.SearchAsync(query, size);
        return Ok(results);
    }
}