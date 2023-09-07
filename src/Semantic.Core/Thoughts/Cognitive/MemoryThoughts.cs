using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive
{
	/// <summary>
	/// Memory thoughts.
    ///
    /// TODO: Move consts to IOptions approach. 
	/// </summary>
	public class MemoryThoughts : BaseThought
    {
        /// <summary>
        /// The collection to use for Memory storage.
        /// </summary>
        private readonly string COLLECTION_PREFIX = "frostaura-semantic-core";
        /// <summary>
        /// The overlap to use when chunking memories.
        /// </summary>
        private readonly int OVERLAP = 200;
        /// <summary>
        /// The chunk size to use when chunking memories.
        /// </summary>
        private readonly int CHUNK_SIZE = 1000;
        /// <summary>
        /// The count of top memories to include from a search.
        /// </summary>
        private readonly int TOP_K = 5;
        /// <summary>
        /// The semantic kernel to use.
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="kernel">The semantic kernel to use.</param>
        /// <param name="logger">Instance logger.</param>
        public MemoryThoughts(IKernel kernel, ILogger<MemoryThoughts> logger)
            :base(logger)
        {
            _kernel = kernel.ThrowIfNull(nameof(kernel));
        }

        /// <summary>
        /// Remember something for future reference.
        /// </summary>
        /// <param name="memory">The Memory to record.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Remember something for future reference.")]
        public async Task<string> CommitToMemoryAsync(
            [Description("The Memory to record.")] string memory,
            [Description("The source of the Memory. A default value of 'general' is acceptable when unsure.")] string source,
            CancellationToken token = default)
        {
            var collection = COLLECTION_PREFIX;
            var chunks = GetTextChunks(memory.ThrowIfNullOrWhitespace(nameof(memory)), CHUNK_SIZE, OVERLAP);
            var MemoryRecordingTasks = chunks
                .Select(async c => await _kernel.Memory.SaveInformationAsync(
                    collection,
                    c.Value,
                    $"{c.Key}.{Guid.NewGuid()}",
                    description: $"Source: {source.ThrowIfNullOrWhitespace(nameof(source))}",
                    cancellationToken: token));

            var response = await Task.WhenAll(MemoryRecordingTasks);
            var responseString = JsonConvert.SerializeObject(response, Formatting.Indented);

            return responseString;
        }

        /// <summary>
        /// Look up something from Memory that was previously remembered.
        /// </summary>
        /// <param name="query">The Memory to search for.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>A collection of the closest matching memories.</returns>
        [SKFunction, Description("Look up something from Memory that was previously remembered.")]
        public async Task<string> RecallFromMemoryAsync(
            [Description("The Memory to search for.")] string query,
            CancellationToken token = default)
        {
            var collection = COLLECTION_PREFIX;
            var memories = _kernel
                .Memory
                .SearchAsync(collection, query.ThrowIfNullOrWhitespace(nameof(query)), TOP_K, cancellationToken: token)
                .GetAsyncEnumerator();
            var result = new List<MemoryRecordMetadata>();

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

        /// <summary>
        /// Look up something from a large text blob using in-Memory vector search.
        /// </summary>
        /// <param name="searchPhrase">The search phrase to lookup.</param>
        /// <param name="largeTextBlob">The large text blob to search.</param>
        /// <param name="resultsCount">The count of results to return.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>A collection (X resultsCount) of semantically most similar results.</returns>
        [SKFunction, Description("Look up something from a large text blob using in-Memory vector search.")]
        public async Task<string> SearchTextBlobAsync(
            [Description("The search phrase to lookup.")] string searchPhrase,
            [Description("The large text blob to search.")] string largeTextBlob,
            [Description("The count of results to return.")] string resultsCount,
            CancellationToken token = default)
        {
            var chunks = GetTextChunks(largeTextBlob.ThrowIfNullOrWhitespace(nameof(largeTextBlob)), CHUNK_SIZE, OVERLAP);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Split a large text into chunks of a max of chunkSize and allow for overlap between the chunks which means slight data repetition but to avoid losing context.
        /// </summary>
        /// <param name="text">The large text to split.</param>
        /// <param name="chunkSize">The size of the chunks' characters. Typically around 1000.</param>
        /// <param name="overlap">The overlap between text which meas the bleedover of context. Typically around 200.</param>
        /// <returns>A collection of key/value pairs where the key is the iteration number of the chunk and the value is the text body of that chunk.</returns>
        private Dictionary<int, string> GetTextChunks(string text, int chunkSize, int overlap)
        {
            var chunks = new Dictionary<int, string>();

            if (text.Length <= chunkSize)
            {
                chunks.Add(0, text);
                return chunks;
            }

            int iterations = (int)Math.Ceiling((double)(text.Length) / (chunkSize - overlap));

            for (int i = 0; i < iterations; i++)
            {
                int startIndex = i * (chunkSize - overlap);
                int length = Math.Min(chunkSize, text.Length - startIndex);
                string chunk = text.Substring(startIndex, length);
                chunks.Add(i, chunk);
            }

            return chunks;
        }
    }
}
