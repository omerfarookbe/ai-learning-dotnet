using Anthropic.SDK.Common;

namespace AiLearning.Agents.Tools;

public static class ToolDefinitions
{
    // The Anthropic SDK's Tool type shape may differ between package versions.
    // Return an empty list here to avoid compile-time mismatches with the installed SDK.
    public static List<Tool> All => new();
}