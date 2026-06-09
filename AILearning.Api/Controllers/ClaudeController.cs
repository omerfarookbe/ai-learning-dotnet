using AiLearning.Api.Models;
using AiLearning.Api.Services.Implementation;
using AiLearning.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaudeController : ControllerBase
    {
        private readonly ILlmService _service;

        public ClaudeController(ILlmService service)
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
}
