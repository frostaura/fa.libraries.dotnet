using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Enumerations.Semantic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Newtonsoft.Json;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive
{
	/// <summary>
	/// Language model thoughts.
	/// </summary>
	public class LanguageModelThoughts : BaseThought
    {
        /// <summary>
        /// The semantic kernel to use.
        /// </summary>
        private readonly Kernel _kernel;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="kernel">The semantic kernel to use.</param>
        /// <param name="logger">Instance logger.</param>
        public LanguageModelThoughts(Kernel kernel, ILogger<LanguageModelThoughts> logger)
            :base(logger)
        {
            _kernel = kernel.ThrowIfNull(nameof(kernel));
        }

        /// <summary>
        /// Prompt a smaller large language model. Ideal for fast responses.
        /// </summary>
        /// <param name="prompt">The LLM prompt.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [KernelFunction, Description("Prompt a smaller large language model. Ideal for fast responses.")]
        public Task<string> PromptSmallLLMAsync(
            [Description("The LLM prompt.")] string prompt,
            CancellationToken token = default)
        {
            var chatSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.5,
                MaxTokens = 4000
            };

            return PromptAsync(prompt.ThrowIfNullOrWhitespace(nameof(prompt)), ModelType.SmallLLM, chatSettings, token);
        }

        /// <summary>
        /// Prompt a large language model. Ideal for fast responses.
        /// </summary>
        /// <param name="prompt">The LLM prompt.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [KernelFunction, Description("Prompt a large language model. Ideal for responses that require superior reasoning.")]
        public Task<string> PromptLLMAsync(
            [Description("The LLM prompt.")] string prompt,
            CancellationToken token = default)
        {
            var chatSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.5,
                MaxTokens = 16000
            };

            return PromptAsync(prompt.ThrowIfNullOrWhitespace(nameof(prompt)), ModelType.LargeLLM, chatSettings, token);
        }

        /// <summary>
        /// Get the embeddings for a string using the an embedding model. Useful for vector search applications.
        /// </summary>
        /// <param name="input">The input text to embed.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The embeddings for the input text.</returns>
        [KernelFunction, Description("Get the embeddings for a string using the an embedding model. Useful for vector search applications.")]
        public async Task<string> GetStringEmbeddingsAsync(
            [Description("The input text to embed.")] string input,
            CancellationToken token)
        {
            var model = _kernel.Services.GetService<ITextEmbeddingGenerationService>();
            var response = await model.GenerateEmbeddingAsync(input.ThrowIfNullOrWhitespace(nameof(input)), cancellationToken: token);

            throw new NotImplementedException("Write tests");

            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Prompt an LLM.
        /// </summary>
        /// <param name="prompt">The LLM prompt.</param>
        /// <param name="llmType">The type of LLM to prompt.</param>
        /// <param name="settings">Chat settings.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The LLM response.</returns>
        private async Task<string> PromptAsync(string prompt, ModelType llmType, OpenAIPromptExecutionSettings settings, CancellationToken token)
        {
            var promptFunction = _kernel.CreateFunctionFromPrompt(prompt, settings);
            var response = await promptFunction.InvokeAsync<string>(_kernel, cancellationToken: token);

            return response;
        }
    }
}
