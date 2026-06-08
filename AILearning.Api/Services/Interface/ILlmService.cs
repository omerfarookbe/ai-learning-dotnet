using AiLearning.Api.Models;

namespace AiLearning.Api.Services.Interface
{
    public interface ILlmService
    {
        Task<ChatResponse> ChatAsync(ChatRequest request);
    }
}
