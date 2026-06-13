using AiLearning.Agents;

var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
    ?? throw new InvalidOperationException("Set ANTHROPIC_API_KEY");

var agent = new ReActAgent(apiKey);

// These questions require different tools
var questions = new[]
{
    "What percentage of people use mobile banking daily, and how many people is that if we surveyed 500?",
    "What do consumers think about sustainability and brand preference?",
    "What is today's date and what year will it be in 5 years?"
};

foreach (var question in questions)
{
    await agent.RunAsync(question);
    Console.WriteLine("=".PadRight(60, '='));
}