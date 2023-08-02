using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semantic.Attributes.Skills;
using Semantic.Consts.Cognitive;
using Semantic.Extensions.String;
using Semantic.Interfaces;
using Semantic.Models.LLMs;
using Semantic.Models.LLMs.OpenAI;
using Semantic.Models.Memory;
using static System.Net.Mime.MediaTypeNames;

namespace Semantic.Skills.Cognitive
{
    /// <summary>
    /// Allows for passing {PromptVariables.INPUT}, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill.
    /// </summary>
    public class CommitToMemorySkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A text blob to commit to memory / our knowledge database.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Allows for passing {PromptVariables.INPUT}, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill. This skill is for extending LLMs with knowledge.";

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="availableSkills">All available skills.</param>
        public CommitToMemorySkill(IEnumerable<BaseSkill> availableSkills, ILogger logger)
            : base(availableSkills, logger)
        { }

        /// <summary>
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        /// <param name="input">The input string to pass to the large language model.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [SkillRequiredContextVariable(PromptVariables.SOURCE, $"This describes the source of the {PromptVariables.INPUT}. For example a website URL or a filename. Use a default value of \"general\", if unsure.")]
        [SkillRequiredContextVariable(PromptVariables.CHUNK_SIZE, $"The count of characters that should be used to split the {PromptVariables.INPUT} into memory chunks. This is important due to the limitations in LLM context window sizes. Use a default value of 1000, if unsure.")]
        [SkillRequiredContextVariable(PromptVariables.OVERLAP_SIZE, $"The count of characters to overlap and get appended to the original split of a chunk. I.e. If {PromptVariables.CHUNK_SIZE} = 1000 & {PromptVariables.OVERLAP_SIZE} = 200 then chunk 2's start would be 800 and its end would be 2000. Use a default value of 200, if unsure.")]
        public override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            var memories = await GetMemoriesFromTextBlobAsync(input, context, token);

            // TODO: Move config to IOptions injection.
            var pineconeEnvironment = "us-east-1-aws";
            var pineconeApiKey = "aad6ee60-dd2e-4386-9732-409b45035e3b";
            var pineconeIndexName = "frostaura-nb001-ada002-5a5764b";
            var pineconeNamespace = "frostaura.zeus.memories";
            var url = $"https://{pineconeIndexName}.svc.{pineconeEnvironment}.pinecone.io/vectors/upsert";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Api-Key", pineconeApiKey);

                var data = new
                {
                    vectors = memories
                        .Select(m => new
                        {
                            id = m.Metadata.Id,
                            values = m.Embeddings,
                            metadata = m.Metadata
                        })
                        .ToArray(),
                    @namespace = pineconeNamespace
                };

                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content, token);
                var responseContent = await response.Content.ReadAsStringAsync(token);

                if (response.IsSuccessStatusCode)
                {
                    return responseContent;
                }
                else
                {
                    throw new ExecutionEngineException($"Failed to record memories to Pinecone.{Environment.NewLine}{responseContent}");
                }
            }
        }

        /// <summary>
        /// Get a collection of memories, given a text blob.
        /// </summary>
        /// <param name="text">Text blob to get memories from.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>A collection of memories for the text blob.</returns>
        private async Task<List<Memory>> GetMemoriesFromTextBlobAsync(string text, Dictionary<string, string> context, CancellationToken token)
        {
            var source = context[PromptVariables.SOURCE];
            var chunkSize = int.Parse(context[PromptVariables.CHUNK_SIZE]);
            var overlapSize = int.Parse(context[PromptVariables.OVERLAP_SIZE]);
            var chunks = text.ToChunks(chunkSize, overlapSize);
            var memoriesTasks = new List<Task<Memory>>();

            for (int i = 0; i < chunks.Count; i++)
            {
                memoriesTasks.Add(GetMemoryFromChunkAsync(chunks[i], context, token));
            }

            var response = await Task.WhenAll(memoriesTasks);

            for (int i = 0; i < response.Length; i++)
            {
                var memory = response[i];

                memory.Metadata.ChunkOverlap = overlapSize;
                memory.Metadata.ChunkSize = chunkSize;
                memory.Metadata.IndexInChunk = i;
                memory.Metadata.Source = source;
            }

            return response
                .ToList();
        }

        /// <summary>
        /// Get the memory representation for a chunk.
        /// </summary>
        /// <param name="text">The text content of the chunk.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The memory representation of the chunk.</returns>
        private async Task<Memory> GetMemoryFromChunkAsync(string text, Dictionary<string, string> context, CancellationToken token)
        {
            var embeddingSkill = _availableSkills.First(s => s is EmbeddingSkill);
            var meta = new MemoryMetadata
            {
                Text = text
            };
            var embedding = JsonConvert.DeserializeObject<List<double>>(await embeddingSkill.RunAsync(text, context, token));
            var memory = new Memory
            {
                Metadata = meta,
                Embeddings = embedding
            };

            return memory;
        }
    }
}