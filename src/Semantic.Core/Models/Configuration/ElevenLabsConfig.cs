using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for ElevenLabs the API.
/// </summary>
[DebuggerDisplay("{ApiKey}")]
public class ElevenLabsConfig
{
	/// <summary>
	/// The API key to use when connecting to the service.
	/// </summary>
	public string ApiKey { get; set; }
}
