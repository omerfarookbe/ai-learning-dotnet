namespace AiLearning.Api.Models
{
    public class ChatResponse
    {
        public string Provider { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
    }
}
