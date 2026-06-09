using System;

namespace AiLearning.Api.Services.Exceptions
{
    // Base exception for LLM service related errors
    public class LlmServiceException : Exception
    {
        public LlmServiceException() { }
        public LlmServiceException(string message) : base(message) { }
        public LlmServiceException(string message, Exception inner) : base(message, inner) { }
    }

    // Thrown when provider configuration (API keys, settings) is missing or invalid
    public class ProviderConfigurationException : LlmServiceException
    {
        public ProviderConfigurationException() { }
        public ProviderConfigurationException(string message) : base(message) { }
        public ProviderConfigurationException(string message, Exception inner) : base(message, inner) { }
    }

    // Thrown when the provider call fails at runtime (network, SDK errors, unexpected responses)
    public class ProviderRuntimeException : LlmServiceException
    {
        public ProviderRuntimeException() { }
        public ProviderRuntimeException(string message) : base(message) { }
        public ProviderRuntimeException(string message, Exception inner) : base(message, inner) { }
    }
}
