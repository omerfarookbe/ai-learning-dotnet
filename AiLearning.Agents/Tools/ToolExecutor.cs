using System.Data;
using System.Text.Json;

namespace AiLearning.Agents.Tools;

public class ToolExecutor
{
    // Simulated survey database — in production this hits your vector store
    private static readonly Dictionary<string, string> SurveyData = new()
    {
        ["mobile banking"] = "Survey March 2024 (n=500): 78% use mobile banking daily. Top concerns: security 45%, ease of use 38%.",
        ["sustainability"] = "Focus group 2024: 65% of consumers pay premium for sustainable products. Strongest in 18-35 age group.",
        ["brand preference"] = "Brand tracker Q1 2024: Nike 34%, Adidas 28%. Brand loyalty strongest in 18-24 demographic.",
        ["data privacy"] = "Sentiment analysis 2024: 82% concerned about data privacy. Only 23% trust social media platforms.",
        ["meal kit"] = "Concept test 2024: Meal kit scored 7.2/10 on purchase intent. Key drivers: convenience 72%, healthy options 61%."
    };

    public string Execute(string toolName, JsonElement input)
    {
        Console.WriteLine($"  [TOOL CALL] {toolName}({input})");

        return toolName switch
        {
            "survey_lookup" => ExecuteSurveyLookup(input),
            "calculator" => ExecuteCalculator(input),
            "current_date" => DateTime.Now.ToString("MMMM dd, yyyy"),
            _ => $"Unknown tool: {toolName}"
        };
    }

    private string ExecuteSurveyLookup(JsonElement input)
    {
        var topic = input.GetProperty("topic").GetString()?.ToLower() ?? "";

        // Find best matching topic
        var match = SurveyData
            .FirstOrDefault(kvp => topic.Contains(kvp.Key) || kvp.Key.Contains(topic));

        return match.Value ?? $"No survey data found for topic: {topic}";
    }

    private string ExecuteCalculator(JsonElement input)
    {
        var expression = input.GetProperty("expression").GetString() ?? "";

        try
        {
            // DataTable.Compute handles basic arithmetic safely
            var result = new DataTable().Compute(expression, null);
            return $"{expression} = {result}";
        }
        catch (Exception ex)
        {
            return $"Could not calculate '{expression}': {ex.Message}";
        }
    }
}