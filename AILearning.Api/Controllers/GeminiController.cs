using AiLearning.Api.Models;
using AiLearning.Api.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace AiLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeminiController : ControllerBase
{
    private readonly GeminiService _service;

    public GeminiController(GeminiService service)
    {
        _service = service;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        var response = await _service.ChatAsync(request);
        return Ok(response);
    }
}