AI Learning .NET API

Small demo API that wraps multiple LLM providers (Anthropic/Claude, OpenAI, Google Gemini) for chat-style requests.

Prerequisites
- .NET 10 SDK
- API keys for providers you intend to use

Quick start
1. Configure API keys in AiLearning.Api/appsettings.json (or user secrets/environment):

```json
{
  "LlmProviders": {
    "Anthropic": { "ApiKey": "<ANTHROPIC_KEY>" },
    "OpenAI": { "ApiKey": "<OPENAI_KEY>" },
    "Gemini": { "ApiKey": "<GEMINI_KEY>" }
  }
}
```

2. Run the API
- From solution root: dotnet run --project AiLearning.Api
- In Development environment Swagger UI will be available at https://localhost:{port}/swagger

Endpoints
- POST /api/claude/chat  - chat via Anthropic/Claude
- POST /api/openai/chat  - chat via OpenAI
- POST /api/gemini/chat  - chat via Google Gemini
- GET  /api/health       - basic health check

Request example (JSON)
{
  "message": "Hello",
  "systemPrompt": "You are a helpful assistant",
  "maxTokens": 512,
  "temperature": 0.7
}

Response example
{
  "provider": "OpenAI",
  "content": "...response text...",
  "inputTokens": 12,
  "outputTokens": 34
}

Errors and exception handling
- Services throw typed exceptions found in AiLearning.Api/Services/Exceptions:
  - ProviderConfigurationException: missing/invalid configuration (API keys)
  - ProviderRuntimeException: runtime/provider SDK failures
- Controllers perform basic validation and log errors; unexpected exceptions return ProblemDetails (HTTP 500).
- Consider adding a centralized ExceptionHandlingMiddleware to map exceptions to ProblemDetails in a single place.

Dependency injection notes
- Concrete provider services are registered in Program.cs. To inject a common ILlmService into controllers, register ILlmService to a concrete implementation or add a provider resolver/factory.

Contributing
- Open a PR with changes; keep DI, logging, and exception handling patterns consistent.

License
- No license specified.
