using AiLearning.Rag;

var anthropicKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
    ?? throw new InvalidOperationException("Set ANTHROPIC_API_KEY environment variable");

var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY environment variable");

var embeddingService = new EmbeddingService(openAiKey);
var store = new DocumentStore(embeddingService);
var rag = new RagEngine(store, anthropicKey);

// Simulate Suzy survey research data
Console.WriteLine("Loading documents into vector store...\n");

await store.AddAsync("s1",
    "Survey conducted in March 2024 among 500 US adults aged 18-35. 78% reported using mobile banking apps daily. Primary concerns were security (45%) and ease of use (38%).",
    "survey-2024-banking");

await store.AddAsync("s2",
    "Focus group findings: Younger consumers prefer brands that demonstrate social responsibility. 65% said they would pay a premium for sustainable products.",
    "focus-group-sustainability");

await store.AddAsync("s3",
    "Brand tracker Q1 2024: Nike leads sportswear preference at 34%, followed by Adidas at 28%. Brand loyalty strongest in 18-24 age group.",
    "brand-tracker-q1");

await store.AddAsync("s4",
    "Consumer sentiment analysis: 82% of respondents expressed concern about data privacy. Only 23% trust social media platforms with personal data.",
    "sentiment-privacy");

await store.AddAsync("s5",
    "Product concept test: New subscription meal kit scored 7.2/10 on purchase intent. Key drivers were convenience (72%) and healthy options (61%).",
    "concept-test-mealkit");

Console.WriteLine("\nDocuments loaded. Ready for questions.\n");
Console.WriteLine("=".PadRight(60, '='));

// Ask questions
var questions = new[]
{
    "What do consumers think about data privacy?",
    "Which sportswear brand is most popular?",
    "How do young people feel about sustainability?"
};

foreach (var question in questions)
{
    Console.WriteLine($"\nQUESTION: {question}");
    Console.WriteLine("-".PadRight(60, '-'));
    var answer = await rag.AskAsync(question);
    Console.WriteLine($"\nANSWER: {answer}");
    Console.WriteLine("=".PadRight(60, '='));
}