namespace AiLearning.Rag;

public class Document
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public float[] Vector { get; set; } = Array.Empty<float>();
}