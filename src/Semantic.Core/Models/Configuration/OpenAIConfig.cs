using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for OpenAI models.
/// </summary>
[DebuggerDisplay("{Endpoint}")]
public class OpenAIConfig
{
	/// <summary>
	/// The large LLM to use for reasoning tasks and tasks requiring a large context window.
	/// </summary>
	public string LargeModelName { get; set; }
	/// <summary>
	/// A small LLM to use for rapid responses.
	/// </summary>
	public string SmallModelName { get; set; }
	/// <summary>
	/// The embedding model to use for vector operations.
	/// </summary>
	public string EmbeddingModelName { get; set; }
    /// <summary>
    /// The text to image model to use for vector operations.
    /// </summary>
    public string TextToImageModelName { get; set; }
    /// <summary>
    /// The endpoint to use for the OpenAI service. If null, the public OpenAI server is configured by default.
    /// </summary>
    public string Endpoint { get; set; }
    /// <summary>
    /// The organization id to use when using the public OpenAI servers.
    /// </summary>
    public string? OrgId { get; set; }
    /// <summary>
    /// The API key to use when connecting to the OpenAI service.
    /// </summary>
    public string ApiKey { get; set; }
}
