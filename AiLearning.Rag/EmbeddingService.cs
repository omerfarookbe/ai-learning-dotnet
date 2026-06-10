using OpenAI.Embeddings;

namespace AiLearning.Rag;

public class EmbeddingService
{
    private readonly EmbeddingClient _client;

    public EmbeddingService(string apiKey)
    {
        _client = new EmbeddingClient("text-embedding-3-small", apiKey);
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var result = await _client.GenerateEmbeddingAsync(text);
        return result.Value.ToFloats().ToArray();
    }

    public static float CosineSimilarity(float[] a, float[] b)
    {
        var dotProduct = a.Zip(b, (x, y) => x * y).Sum();
        var magnitudeA = MathF.Sqrt(a.Sum(x => x * x));
        var magnitudeB = MathF.Sqrt(b.Sum(x => x * x));
        return dotProduct / (magnitudeA * magnitudeB);
    }
}