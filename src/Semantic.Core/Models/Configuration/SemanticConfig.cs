namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Composite semantic config.
/// </summary>
public class SemanticConfig
{
    // <summary>
    /// App config.
    /// </summary>
    public AppConfig AppConfig { get; set; }
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
    /// <summary>
    /// Semantic Memory Config
    /// </summary>
    public SemanticMemoryConfig SemanticMemoryConfig { get; set; }
    /// <summary>
    /// Configuration for Medium blogging platform.
    /// </summary>
    public MediumConfig MediumConfig { get; set; }
}
