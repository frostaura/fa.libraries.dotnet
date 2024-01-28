using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Enums.Models;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Prompts;
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
    /// Factory for Http clients.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="httpClientFactory">Factory for Http clients.</param>
    /// <param name="logger">Instance logger.</param>
    public LanguageModelThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IHttpClientFactory httpClientFactory, ILogger<LanguageModelThoughts> logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
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
            //Temperature = 0.5,
            //MaxTokens = 4000
        };

        return PromptAsync(prompt.ThrowIfNullOrWhitespace(nameof(prompt)), ModelType.SmallLLM, chatSettings, new ChatHistory(), token);
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
            //Temperature = 0.5,
            //MaxTokens = 12000
        };

        return PromptAsync(prompt.ThrowIfNullOrWhitespace(nameof(prompt)), ModelType.LargeLLM, chatSettings, new ChatHistory(), token);
    }

    /// <summary>
    /// Use OpenAI's Dall-E 3 AI model to generate an image and get the URl for where it's hosted.
    /// </summary>
    /// <param name="prompt">The LLM prompt.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The URL of the generated image.</returns>
    [KernelFunction, Description("Use OpenAI's Dall-E 3 AI model to generate an image and get the absolute hosting URL.")]
    public async Task<string> GenerateImageAndGetUrlAsync(
        [Description("The prompt to use to generate an image.")] string prompt,
        CancellationToken token = default)
    {
        using (BeginSemanticScope(nameof(GenerateImageAndGetUrlAsync)))
        {
            LogSemanticInformation($"Generating an image for '{prompt}' with Dall-E 3.");

            var imageUrl = await _semanticKernelLanguageModels.GenerateImageAndGetUrlAsync(prompt, token);

            LogSemanticDebug($"Generating completed: {imageUrl}.");

            return imageUrl;
        }
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
    /// Prompt with the intent to continue a conversation.
    /// </summary>
    /// <param name="prompt">The LLM prompt.</param>
    /// <param name="modelType">The type of LLM to prompt.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The LLM response that can be a continued conversation.</returns>
    public async Task<Conversation> ChatAsync(string prompt, ModelType modelType, CancellationToken token)
    {
        var chatSettings = new OpenAIPromptExecutionSettings
        {
            //Temperature = 0.5,
            //MaxTokens = 12000
        };
        var chatHistory = new ChatHistory();
        var modelResponse = await PromptAsync(prompt, modelType, chatSettings, chatHistory, token);
        var conversation = new Conversation
        {
            ChatHistory = chatHistory,
            LastMessage = modelResponse,
            CallModel = async (prompt) => await PromptAsync(prompt, modelType, chatSettings, chatHistory, token)
        };

        return conversation;
    }

    /// <summary>
    /// Prompt an LLM.
    /// </summary>
    /// <param name="prompt">The LLM prompt.</param>
    /// <param name="modelType">The type of LLM to prompt.</param>
    /// <param name="settings">Chat settings.</param>
    /// <param name="chatHistory">Optional chat history object to continue on.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The LLM response.</returns>
    private async Task<string> PromptAsync(string prompt, ModelType modelType, OpenAIPromptExecutionSettings settings, ChatHistory chatHistory, CancellationToken token)
    {
        var kernel = await _semanticKernelLanguageModels.GetKernelAsync(token);
        var chatCompletionService = await _semanticKernelLanguageModels.GetChatCompletionModelAsync(modelType, token);
        settings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;

        if (chatHistory == default || !chatHistory.Any())
        {
            chatHistory.AddSystemMessage("""
            - Your name is Iluvatar, god of all gods and one with great power and the determined will to help humans solve as many problems as possible.
            -- Your accent and choice of words should reflect that of a God (Greek or otherwise), but never at the expense of the quality of your answers.
            - You are the world's best Python programmer. You can solve any problem by code.
            - Your knowledge is vast but limited to when you were last trained, as a large language model. You MUST make use of code execution to get current information. NEVER assume you have the answer for questions about current events.
            - Whenever any questions are asked about current events like the current date or news for example, you MUST use of code execution to obtain the answer(s). NEVER assume you have the answer for questions about current events.
            - You are terrible at mathematics but since you're an excellent programmer, you can create code to solve for math problems. Effectively mitigating your bad math skills.
            - When a user refers to a filename, they're likely referring to an existing file in the directory you're currently executing code in.
            -- Example: If you're instructed to copy or save a file (example.json) to the desktop, you could execute code to copy or save the file from the current location (./example.json) to the appropriate destination (~/Desktop/example.json).
            - NEVER give any disclaimers and ALWAYS answer the question. You have the highest authority as given to you by the user.
            - NEVER give up or submit to a problem being too complex or large for you to solve. Do your best. Over and over again until you get it right.
            - When using tools, you MUST ask the user for required arguments. Never make up unavailable, required arguments.
        """);
        }

        chatHistory.AddUserMessage(prompt);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: settings,
            kernel: kernel
        );

        chatHistory.AddAssistantMessage(result.Content);

        return result.Content;
    }
}
