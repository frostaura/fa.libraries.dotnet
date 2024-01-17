using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for Pexels the API.
/// </summary>
[DebuggerDisplay("{ApiKey}")]
public class PexelsConfig
{
	/// <summary>
	/// The API key to use when connecting to the Pexels service.
	/// </summary>
	public string ApiKey { get; set; }
}
