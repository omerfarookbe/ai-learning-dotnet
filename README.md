AI Learning .NET API

Small demo API that wraps multiple LLM providers (Anthropic/Claude, OpenAI, Google Gemini) for chat-style requests.

Recent changes
- Added new module: AiLearning.Rag (RAG solution) to support retrieval-augmented generation scenarios.
- Added typed service exceptions (AiLearning.Api/Services/Exceptions):
  - LlmServiceException (base)
  - ProviderConfigurationException (missing/invalid config, e.g. API keys)
  - ProviderRuntimeException (provider SDK/network/runtime failures)
- Services (ClaudeService, OpenAIService, GeminiService) now validate configuration and throw the typed exceptions and wrap provider SDK calls.
- Controllers perform request validation, logging, and catch exceptions. They currently depend on concrete provider services (ClaudeService, OpenAIService, GeminiService) that are registered in Program.cs.

Prerequisites
- .NET 10 SDK
- API keys for providers you intend to use

Configuration
Configure API keys in AiLearning.Api/appsettings.json, user secrets, or environment variables:

```json
{
  "LlmProviders": {
    "Anthropic": { "ApiKey": "<ANTHROPIC_KEY>" },
    "OpenAI": { "ApiKey": "<OPENAI_KEY>" },
    "Gemini": { "ApiKey": "<GEMINI_KEY>" }
  }
}
```

Run
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
- Services throw the typed exceptions listed above. Controllers log and return ProblemDetails (HTTP 500) for unexpected errors.
- ProviderConfigurationException indicates missing or invalid configuration (register API keys).
- ProviderRuntimeException wraps runtime errors from provider SDKs or network issues.

Dependency injection notes
- Current controller constructors accept concrete provider services (ClaudeService, OpenAIService, GeminiService) because Program.cs registers those concrete types.
- If you prefer controllers to depend on ILlmService, update Program.cs to register the interface to a concrete implementation, e.g.:

```csharp
// single implementation
builder.Services.AddSingleton<ILlmService, ClaudeService>();

// or factory / resolver to choose provider at runtime
builder.Services.AddSingleton<ProviderResolver>();
```

- For multiple providers a recommended pattern is to:
  - Register each concrete provider (AddSingleton<ClaudeService>(), etc.)
  - Register an IProviderResolver or factory service that picks the correct provider by name/config and returns ILlmService
  - Controllers or a higher-level service request the resolver to obtain the appropriate ILlmService

Logging
- Controllers use ILogger<T> to emit warnings and error logs. Services do not log provider internals; they throw ProviderRuntimeException which can be logged by controllers or middleware.

Recommended next steps
- Add a centralized ExceptionHandlingMiddleware to map typed exceptions to standardized ProblemDetails responses (400/422/500/503 as appropriate).
- Implement a ProviderResolver/factory if you need controller code to remain interface-driven and choose providers at runtime.

Contributing
- Open a PR with changes; keep DI, logging, and exception handling patterns consistent.

License
- No license specified.

Embeddings program (current status)
----------------------------------
There is a separate console project AiLearning.Embeddings (AiLearning.Embeddings/Program.cs) included in the solution. Current behavior:

- Uses OpenAI.Embeddings with model "text-embedding-3-small".
- Demo generates embeddings for three sample sentences, prints vector dimensions and the first 5 values, and computes cosine similarity between each pair.
- API key is required and must be provided either as the first command-line argument or via the environment variable OpenAI_API_KEY.

How to run the demo

- From solution root (example passing key on the command line):
  dotnet run --project AiLearning.Embeddings -- <OPENAI_API_KEY>

- Or set the environment variable and run without arguments:
  $env:OpenAI_API_KEY = "<your_key>"  # PowerShell example
  dotnet run --project AiLearning.Embeddings

Notes

- The embeddings project is a standalone demo and is not currently integrated with AiLearning.Api. Use it as a reference when implementing embedding generation or when adding an embedding-backed vector store to the API.

RAG module (AiLearning.Rag)
---------------------------

There is a new project AiLearning.Rag included in the solution. This module demonstrates Retrieval-Augmented Generation (RAG) patterns and shows how to combine a vector store of embeddings with an LLM to answer queries using retrieved context.

Current status
- Contains example code to index documents, generate embeddings, and perform retrieval to provide context to LLM prompts.
- May depend on AiLearning.Embeddings for embedding generation or include its own embedding wrapper — check the AiLearning.Rag project for implementation details.

How to run
- Build the solution: dotnet build
- Run examples or the RAG demo project (if it's a console/sample): dotnet run --project AiLearning.Rag

If you want more detailed README instructions about the RAG module (architecture diagram, supported vector stores, sample documents, or configuration), tell me what to include and I will expand it.
