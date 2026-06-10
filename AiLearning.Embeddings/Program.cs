using OpenAI.Embeddings;

var apiKey = args.Length > 0  ? args[0] : 
    Environment.GetEnvironmentVariable("OpenAI_API_KEY") ?? 
    throw new ArgumentException("API key must be provided as a command line argument or set in the environment variable 'OpenAI_API_KEY'.");

var client = new EmbeddingClient("text-embedding-3-small", apiKey);

// Three sentences - two similar and one different
var sentences = new[]
{
    "The dog chased the ball across the park.",
    "A puppy ran after a ball in the garden.",
    "The quarterly earnings report exceeded expectations."
};

Console.WriteLine("Generating embeddings...");

var embeddings = new List<float[]>();

foreach(var sentence in sentences)
{
    var resultEmbedding = client.GenerateEmbedding(sentence);
    var vector = resultEmbedding.Value.ToFloats().ToArray();
    embeddings.Add(vector);

    // Print the first 5 numbers so you can see what a vector looks like
    Console.WriteLine($"Text: \"{sentence}\"");
    Console.WriteLine($"Vector dimensions: {vector.Length}");
    Console.WriteLine($"First 5 values: [{string.Join(", ", vector.Take(5).Select(v => v.ToString("F4")))}]");
    Console.WriteLine();
}

// Now compute cosine similarity between each pair
Console.WriteLine("=== Similarity Scores ===\n");

for (int i = 0; i < sentences.Length; i++)
{
    for (int j = i + 1; j < sentences.Length; j++)
    {
        var similarity = CosineSimilarity(embeddings[i], embeddings[j]);
        Console.WriteLine($"Sentence {i + 1} vs Sentence {j + 1}: {similarity:F4}");
        Console.WriteLine($"  \"{sentences[i]}\"");
        Console.WriteLine($"  \"{sentences[j]}\"");
        Console.WriteLine();
    }
}

static float CosineSimilarity(float[] a, float[] b)
{
    // Dot product divided by product of magnitudes
    // Result is between -1 and 1
    // 1 = identical meaning, 0 = unrelated, -1 = opposite
    var dotProduct = a.Zip(b, (x, y) => x * y).Sum();
    var magnitudeA = MathF.Sqrt(a.Sum(x => x * x));
    var magnitudeB = MathF.Sqrt(b.Sum(x => x * x));
    return dotProduct / (magnitudeA * magnitudeB);
}