using System.Diagnostics;
using Newtonsoft.Json;

namespace Semantic.Models.LLMs.OpenAI
{
    /// <summary>
    /// The root response object from the OpenAI API embedding model.
    /// </summary>
    [DebuggerDisplay("{Model}")]
    public class OpenAIEmbeddingResponse
    {
        /// <summary>
        /// The name of the model used.
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// The embedding data lines.
        /// </summary>
        public List<OpenAIEmbeddingData> Data { get; set; }
    }
}