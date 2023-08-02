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
using Microsoft.SemanticKernel.Memory;

namespace Semantic.Core.Functions.Semantic
{
    /// <summary>
    /// Allows for passing an input, a text blob that should be used to query a knowledge base (typically a vector database like Pinecone).
    /// </summary>
    public class RecallFromMemory : FunctionCore
	{
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Allows for passing an input, a text blob that should be used to query the knowledge base (typically a vector database like Pinecone)";
        /// <summary>
        /// The semantic kernel to use for LLM calls.
        /// </summary>
        private readonly IKernel _kernel;
        /// <summary>
        /// The collection to use for memory storage.
        /// </summary>
        private readonly string COLLECTION_PREFIX = "frostaura-semantic-core";
        /// <summary>
        /// The count of top memories to include from a search.
        /// </summary>
        private readonly int TOP_K = 5;
        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="kernel">The semantic kernel to use for LLM calls.</param>
        /// <param name="logger">Instance logger.</param>
        public RecallFromMemory(IKernel kernel, ILogger<RecallFromMemory> logger)
            : base(logger)
        {
            _kernel = kernel
                .ThrowIfNull(nameof(kernel));
        }

        /// <summary>
        /// Allows for passing an input, a text blob that should be used to query a knowledge base (typically a vector database like Pinecone).
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.INPUT, "A text blob to query the knowledge database with for similar memories to this query.")]
        public override async Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            var input = arguments.GetArgument(ArgumentNames.INPUT);
            var collection = COLLECTION_PREFIX;
            var memories = _kernel
                .Memory
                .SearchAsync(collection, input, TOP_K, cancellationToken: token)
                .GetAsyncEnumerator();
            var result = new List<MemoryRecordMetadata>(); // Change 'Memory' to the actual type of your items

            try
            {
                while (await memories.MoveNextAsync())
                {
                    var memory = memories.Current;

                    result.Add(memory.Metadata);
                }
            }
            finally
            {
                await memories.DisposeAsync();
            }

            var resultStr = JsonConvert.SerializeObject(result, Formatting.Indented);

            return resultStr;
        }
    }
}
