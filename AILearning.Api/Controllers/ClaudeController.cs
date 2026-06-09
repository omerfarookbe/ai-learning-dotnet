using AiLearning.Api.Models;
using AiLearning.Api.Services.Implementation;
using AiLearning.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AiLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaudeController : ControllerBase
    {
        private readonly ClaudeService _service;
        private readonly ILogger<ClaudeController> _logger;

        public ClaudeController(ClaudeService service, ILogger<ClaudeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Chat request was null.");
                return BadRequest("Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Chat request message was empty.");
                return BadRequest("Message is required.");
            }

            try
            {
                var response = await _service.ChatAsync(request);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Configuration error while calling Claude service.");
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while calling Claude service.");
                return Problem(detail: "An unexpected error occurred while processing the request.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

    }
}
