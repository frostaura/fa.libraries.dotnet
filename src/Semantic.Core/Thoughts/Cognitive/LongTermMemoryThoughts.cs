using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Extensions.String;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Newtonsoft.Json;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;

/// <summary>
/// Memory thoughts.
/// </summary>
public class LongTermMemoryThoughts : BaseThought
{
    /// <summary>
    /// Configuration for semantic memory.
    /// </summary>
    private readonly SemanticMemoryConfig _semanticMemoryConfig;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="semanticMemoryConfig">Configuration for semantic memory.</param>
    /// <param name="logger">Instance logger.</param>
    public LongTermMemoryThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IOptions<SemanticMemoryConfig> semanticMemoryConfig, ILogger<LongTermMemoryThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _semanticMemoryConfig = semanticMemoryConfig
            .ThrowIfNull(nameof(semanticMemoryConfig))
            .Value
            .ThrowIfNull(nameof(semanticMemoryConfig));
    }

    /// <summary>
    /// Remember something for future reference.
    /// </summary>
    /// <param name="memory">The Memory to record.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Commit something to long-term memory for future reference.")]
    public async Task<string> CommitToMemoryAsync(
        [Description("The memory to record.")] string memory,
        [Description("The source of the memory. A default value of 'general' is acceptable when unsure.")] string source,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(CommitToMemoryAsync)))
        {
            _logger.LogInformation("Committing long-term memory '{Memory}' to persistent store.", memory);
            _logger.LogDebug("Chunking the memory. Source: {Source}", source);

            var chunks = memory
                .ThrowIfNullOrWhitespace(nameof(memory))
                .ChunkByDelimiterAfterCharsCount();
            var memoryStore = await _semanticKernelLanguageModels.GetSemanticTextMemoryAsync(token);

            _logger.LogDebug("Starting to persist memory chunks.");

            var memoryRecordingTasks = chunks
                .Select(async c => await memoryStore.SaveInformationAsync(
                    _semanticMemoryConfig.CollectionName,
                    c,
                    $"{Guid.NewGuid()}",
                    description: $"Source: {source.ThrowIfNullOrWhitespace(nameof(source))}",
                    cancellationToken: token));

            _logger.LogDebug("Persisting memory chunks succeeded.");

            var response = await Task.WhenAll(memoryRecordingTasks);
            var responseString = JsonConvert.SerializeObject(response, Formatting.Indented);

            _logger.LogInformation("Successfully committed the memory for the long-term.");

            return responseString;
        }
    }

    /// <summary>
    /// Look up something from long-term memory that was previously recorded.
    /// </summary>
    /// <param name="query">The Memory to search for.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>A collection of the closest matching memories.</returns>
    [KernelFunction, Description("Look up something from long-term memory that was previously recorded.")]
    public async Task<string> RecallFromMemoryAsync(
        [Description("Query for the long-term memory store.")] string query,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(RecallFromMemoryAsync)))
        {
            _logger.LogInformation("Retrieving the top {TopK} memories from long-term store for query: {Query}", _semanticMemoryConfig.TopK, query);

            var memoryStore = await _semanticKernelLanguageModels.GetSemanticTextMemoryAsync(token);
            var memories = memoryStore
                .SearchAsync(_semanticMemoryConfig.CollectionName, query.ThrowIfNullOrWhitespace(nameof(query)), _semanticMemoryConfig.TopK, cancellationToken: token)
                .GetAsyncEnumerator();
            var result = new List<MemoryRecordMetadata>();

            _logger.LogDebug("Successfully fetched memories from the store.");

            try
            {
                while (await memories.MoveNextAsync())
                {
                    var MemoryResult = memories.Current;

                    result.Add(MemoryResult.Metadata);
                }
            }
            finally
            {
                await memories.DisposeAsync();
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
