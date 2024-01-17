using FrostAura.Libraries.Semantic.Core.Enums.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;

namespace FrostAura.Libraries.Semantic.Core.Interfaces.Data
{
	/// <summary>
	/// A component for communicating with language models.
	/// </summary>
	public interface ISemanticKernelLanguageModelsDataAccess
    {
        /// <summary>
        /// Create a default text embedding generation service instance.
        /// </summary>
        /// <param name="token">Token to allow for cancelling downstream operations.</param>
        /// <returns>Text embedding generation service instance.</returns>
        Task<ITextEmbeddingGenerationService> GetEmbeddingModelAsync(CancellationToken token);
        /// <summary>
        /// Create a text generation service instance.
        /// </summary>
        /// <param name="modelType">The model type to communicate with.</param>
        /// <param name="token">Token to allow for cancelling downstream operations.</param>
        /// <returns>Chat model instance.</returns>
        Task<IChatCompletionService> GetChatCompletionModelAsync(ModelType modelType, CancellationToken token);
        /// <summary>
        /// Get a semantic memory store.
        /// </summary>
        /// <param name="token">Token to allow for cancelling downstream operations.</param>
        /// <returns>Semantic memory instance.</returns>
        Task<ISemanticTextMemory> GetSemanticTextMemoryAsync(CancellationToken token);
        /// <summary>
        /// Generate an image with a generative AI model and return the URL where the image is hosted.
        /// </summary>
        /// <param name="prompt">The promt for the generation.</param>
        /// <param name="token">Token to allow for cancelling downstream operations.</param>
        /// <returns>Semantic memory instance.</returns>
        Task<string> GenerateImageAndGetUrlAsync(string prompt, CancellationToken token);
        /// <summary>
        /// Get a default Kernel instance.
        /// </summary>
        /// <param name="token">Token to allow for cancelling downstream operations.</param>
        /// <returns>a Default Kernel instance.</returns>
        Task<Kernel> GetKernelAsync(CancellationToken token);
    }
}
