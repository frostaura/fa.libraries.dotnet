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
        /// <summary>
        /// Pexels API config.
        /// </summary>
        public PexelsConfig PexelsConfig { get; set; }
        /// <summary>
        /// ElevenLabs config.
        /// </summary>
        public ElevenLabsConfig ElevenLabsConfig { get; set; }
        /// <summary>
        /// Google config.
        /// </summary>
        public GoogleConfig GoogleConfig { get; set; }
        /// <summary>
        /// FNB Config
        /// </summary>
        public FNBConfig FNBConfig { get; set; }
    }
}

