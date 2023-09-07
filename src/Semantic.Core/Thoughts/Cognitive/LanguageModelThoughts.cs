using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Enumerations.Semantic;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.SkillDefinition;
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
        private readonly IKernel _kernel;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="kernel">The semantic kernel to use.</param>
        /// <param name="logger">Instance logger.</param>
        public LanguageModelThoughts(IKernel kernel, ILogger<LanguageModelThoughts> logger)
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
        [SKFunction, Description("Prompt a smaller large language model. Ideal for fast responses.")]
        public Task<string> PromptSmallLLMAsync(
            [Description("The LLM prompt.")] string prompt,
            CancellationToken token = default)
        {
            var chatSettings = new ChatRequestSettings
            {
                Temperature = 0.5,
                MaxTokens = 4000
            };

            return PromptAsync(prompt, ModelType.SmallLLM, chatSettings, token);
        }

        /// <summary>
        /// Prompt a large language model. Ideal for fast responses.
        /// </summary>
        /// <param name="prompt">The LLM prompt.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Prompt a large language model. Ideal for responses that require superior reasoning.")]
        public Task<string> PromptLLMAsync(
            [Description("The LLM prompt.")] string prompt,
            CancellationToken token = default)
        {
            var chatSettings = new ChatRequestSettings
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
        [SKFunction, Description("Get the embeddings for a string using the an embedding model. Useful for vector search applications.")]
        public async Task<string> GetStringEmbeddingsAsync(
            [Description("The input text to embed.")] string input,
            CancellationToken token)
        {
            var model = _kernel.GetService<ITextEmbeddingGeneration>(ModelType.Embedding.ToString());
            var response = await model.GenerateEmbeddingAsync(input.ThrowIfNullOrWhitespace(nameof(input)), token);

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
        private async Task<string> PromptAsync(string prompt, ModelType llmType, ChatRequestSettings settings, CancellationToken token)
        {
            var model = _kernel.GetService<IChatCompletion>(llmType.ToString());
            var chat = model.CreateNewChat();

            chat.AddUserMessage(prompt.ThrowIfNullOrWhitespace(nameof(prompt)));

            return await model.GenerateMessageAsync(chat, settings, token);
        }
    }
}
