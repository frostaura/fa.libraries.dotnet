using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration
{
    /// <summary>
    /// Composite semantic config.
    /// </summary>
    [DebuggerDisplay("{OpenAIConfig}, {PineconeConfig}")]
    public class SemanticConfig
    {
        /// <summary>
        /// OpenAI config.
        /// </summary>
        public OpenAIConfig OpenAIConfig { get; set; }
        /// <summary>
        /// Pinecone config.
        /// </summary>
        public PineconeConfig PineconeConfig { get; set; }
    }
}

