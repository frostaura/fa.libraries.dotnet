using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Enums.Models;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Newtonsoft.Json;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;

/// <summary>
/// Language model thoughts.
/// </summary>
public class LanguageModelThoughts : BaseThought
{
    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public LanguageModelThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<LanguageModelThoughts> logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

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
            MaxTokens = 12000
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
        CancellationToken token = default)
    {
        var model = await _semanticKernelLanguageModels.GetEmbeddingModelAsync(token);
        var response = await model.GenerateEmbeddingAsync(input.ThrowIfNullOrWhitespace(nameof(input)), cancellationToken: token);

        return JsonConvert.SerializeObject(response.ToArray());
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
        var kernel = await _semanticKernelLanguageModels.GetKernelAsync(token);
        var chatCompletionService = await _semanticKernelLanguageModels.GetChatCompletionModelAsync(llmType, token);
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        var chatHistory = new ChatHistory();

        chatHistory.AddSystemMessage("""
            - You are offline so whenever any questions are asked about current events like the date or news, you should use functions where possible to obtain current information.
            - You are terrible at mathematics but fantastic at creating Python code for any math questions and can get the correct answer that way.
        """);
        chatHistory.AddUserMessage(prompt);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: promptExecutionSettings,
            kernel: kernel
        );

        return result.Content;
    }
}
