namespace AiLearning.Api.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
        public int MaxTokens { get; set; } = 512;
        public float Temperature { get; set; } = 0.7f;
    }
}
