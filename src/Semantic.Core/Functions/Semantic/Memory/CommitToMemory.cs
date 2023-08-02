using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Polly;
using Polly.Retry;
using Semantic.Core.Attribute.Functions;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Extensions.Functions;
using Microsoft.SemanticKernel.AI.Embeddings;
using Semantic.Core.Enumerations.Semantic;
using Newtonsoft.Json;

namespace Semantic.Core.Functions.Semantic
{
    /// <summary>
    /// Allows for passing an input, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill.
    /// </summary>
    public class CommitToMemory : FunctionCore
	{
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Allows for passing an input, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemory.";
        /// <summary>
        /// The semantic kernel to use for LLM calls.
        /// </summary>
        private readonly IKernel _kernel;
        /// <summary>
        /// The collection to use for memory storage.
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
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="kernel">The semantic kernel to use for LLM calls.</param>
        /// <param name="logger">Instance logger.</param>
        public CommitToMemory(IKernel kernel, ILogger<CommitToMemory> logger)
            : base(logger)
        {
            _kernel = kernel
                .ThrowIfNull(nameof(kernel));
        }

        /// <summary>
        /// Allows for passing an input, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill.
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.INPUT, "A text blob to generate embeddings for.")]
        [Argument(ArgumentNames.SOURCE, $"This describes the source of the input. For example a website URL or a filename. Use a default value of \"general\", if unsure.")]
        public override async Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            var input = arguments.GetArgument(ArgumentNames.INPUT);
            var source = arguments.GetArgument(ArgumentNames.SOURCE);
            var collection = COLLECTION_PREFIX;
            var chunks = GetTextChunks(input, CHUNK_SIZE, OVERLAP);
            var memoryRecordingTasks = chunks
                .Select(async c => await _kernel.Memory.SaveInformationAsync(
                    collection,
                    c.Value,
                    $"{c.Key}.{Guid.NewGuid()}",
                    description: $"Source: {source}",
                    cancellationToken: token));

            var response = await Task.WhenAll(memoryRecordingTasks);
            var responseString = JsonConvert.SerializeObject(response, Formatting.Indented);

            return responseString;
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
